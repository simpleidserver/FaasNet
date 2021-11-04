using FaasNet.Runtime.Builders;
using FaasNet.Runtime.CloudEvent.Models;
using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Persistence;
using FaasNet.Runtime.Persistence.InMemory;
using FaasNet.Runtime.Processors;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FaasNet.Runtime.Tests
{
    public class WorkflowDefinitionFixture : IClassFixture<WebApplicationFactory<FakeStartup>>
    {
        [Fact]
        public async Task When_Run_HelloWorld()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("helloWorld", "v1", "name", "description")
                .StartsWith(o => o.Inject().Data(new { result = "Hello World!" }).End())
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("Hello World!", instance.Output["result"].ToString());
        }

        [Fact]
        public async Task When_Inject_Persons_And_Filter()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("injectAndFilterPersons", "v1", "name", "description")
                .StartsWith(o => o.Inject().Data(new
                {
                    people = new List<dynamic>
                {
                    new
                    {
                        fname = "John",
                        lname = "Doe",
                        address = "1234 SomeStreet",
                        age = 40
                    },
                    new
                    {
                        fname = "Marry",
                        lname = "Allice",
                        address = "1234 SomeStreet",
                        age = 25
                    },
                    new
                    {
                        fname = "Kelly",
                        lname = "Mill",
                        address = "1234 SomeStreet",
                        age = 30
                    }
                }
                }).SetOutputFilter("{ 'people' : '$.people[?(@.age < 40)]' }").End())
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"people\": [\r\n    {\r\n      \"fname\": \"Marry\",\r\n      \"lname\": \"Allice\",\r\n      \"address\": \"1234 SomeStreet\",\r\n      \"age\": 25\r\n    },\r\n    {\r\n      \"fname\": \"Kelly\",\r\n      \"lname\": \"Mill\",\r\n      \"address\": \"1234 SomeStreet\",\r\n      \"age\": 30\r\n    }\r\n  ]\r\n}", instance.OutputStr);
        }

        [Fact]
        public async Task When_Run_GreetingFunction()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("greeting", "v1", "name", "description")
                .AddFunction(o => o.RestAPI("greetingFunction", "http://localhost/swagger/v1/swagger.json#greeting"))
                .StartsWith(o => o.Operation().SetActionMode(WorkflowDefinitionActionModes.Sequential).AddAction("Greet",
                (act) => act.SetFunctionRef("greetingFunction", "{ 'name' : '$.person.name' }")
                .SetActionDataFilter(string.Empty, "$.person.message", "$.result"))
                .End())
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{'person' : { 'name': 'simpleidserver', 'message': 'message' }}");
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"person\": {\r\n    \"name\": \"simpleidserver\",\r\n    \"message\": \"Welcome to Serverless Workflow, simpleidserver!\"\r\n  }\r\n}", instance.OutputStr);
        }

        [Fact]
        public async Task When_Publish_Event()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("greeting", "v1", "name", "description")
                .AddConsumedEvent("GreetingEvent", "greetingEventSource", "greetingEventType")
                .AddFunction(o => o.RestAPI("greetingFunction", "http://localhost/swagger/v1/swagger.json#greeting"))
                .StartsWith(o => o.Event()
                    .SetExclusive(false)
                    .AddOnEvent(
                        onevt => onevt.AddEventRef("GreetingEvent").AddAction("Greet", act => act.SetFunctionRef("greetingFunction", "{ 'name' : '$.person.name' }")).Build()
                    )
                )
                .Build();
            await runtimeJob.RegisterWorkflowDefinition(workflowDefinition);
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            runtimeJob.Start();
            var jObj = JObject.Parse("{'greet': {'name': 'simpleidserver'}}");
            await runtimeJob.Publish(new CloudEventMessage
            {
                Id = "id",
                Source = "greetingEventSource",
                Type = "greetingEventType",
                SpecVersion = "1.0",
                Data = jObj
            });
            string sss = "";
            runtimeJob.Stop();
        }

        public class RuntimeJob
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly IBusControl _busControl;

            public RuntimeJob()
            {
                var factory = new CustomWebApplicationFactory<FakeStartup>();
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddRuntime();
                serviceCollection.AddMassTransitHostedService();
                var mcq = new Mock<Factories.IHttpClientFactory>();
                mcq.Setup(c => c.Build()).Returns(factory.CreateClient());
                serviceCollection.AddSingleton<Factories.IHttpClientFactory>(mcq.Object);
                _serviceProvider = serviceCollection.BuildServiceProvider();
                _busControl = _serviceProvider.GetRequiredService<IBusControl>();
            }

            public Task RegisterWorkflowDefinition(WorkflowDefinitionAggregate workflowDef)
            {
                var repository = _serviceProvider.GetRequiredService<IWorkflowDefinitionRepository>();
                return repository.Add(workflowDef, CancellationToken.None);
            }

            public Task<WorkflowInstanceAggregate> InstanciateAndLaunch(WorkflowDefinitionAggregate workflowDef, string input)
            {
                var runtimeEngine = _serviceProvider.GetRequiredService<IRuntimeEngine>();
                return runtimeEngine.InstanciateAndLaunch(workflowDef, input, CancellationToken.None);
            }

            public void Start()
            {
                _busControl.Start();
            }

            public void Stop()
            {
                _busControl.Stop();
            }

            public Task Publish(CloudEventMessage msg)
            {
                return _busControl.Publish(msg, CancellationToken.None);
            }
        }
    }
}
