using FaasNet.Runtime.Builders;
using FaasNet.Runtime.CloudEvent.Models;
using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Persistence;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task When_Call_GreetingFunction_With_Event()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("greeting", "v1", "name", "description")
                .AddConsumedEvent("GreetingEvent", "greetingEventSource", "greetingEventType")
                .AddFunction(o => o.RestAPI("greetingFunction", "http://localhost/swagger/v1/swagger.json#greeting"))
                .StartsWith(o => o.Event()
                    .SetExclusive(false)
                    .AddOnEvent(
                        onevt => onevt.AddEventRef("GreetingEvent").AddAction("Greet", act => act.SetFunctionRef("greetingFunction", "{ 'name' : '$.person.message' }").SetActionDataFilter(string.Empty, "$.person.message", "$.result")).Build()
                    ).End()
                )
                .Build();
            await runtimeJob.RegisterWorkflowDefinition(workflowDefinition);
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            runtimeJob.Start();
            var jObj = JObject.Parse("{'person': {'message': 'simpleidserver'}}");
            await runtimeJob.Publish(new CloudEventMessage
            {
                Id = "id",
                Source = "greetingEventSource",
                Type = "greetingEventType",
                SpecVersion = "1.0",
                Data = jObj
            });
            instance = runtimeJob.WaitTerminate(instance.Id);
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"person\": {\r\n    \"message\": \"Welcome to Serverless Workflow, simpleidserver!\"\r\n  }\r\n}", instance.OutputStr);
        }

        [Fact]
        public async Task When_Send_Two_Events_And_Set_Exlusive_To_False()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("greeting", "v1", "name", "description")
                .AddConsumedEvent("FirstEvent", "firstEventSource", "firstEventType")
                .AddConsumedEvent("SecondEvent", "secondEventSource", "secondEventType")
                .AddFunction(o => o.RestAPI("greetingFunction", "http://localhost/swagger/v1/swagger.json#greeting"))
                .StartsWith(o => o.Event()
                    .SetExclusive(false)
                    .AddOnEvent(
                        onevt => onevt
                            .AddEventRef("FirstEvent").AddEventRef("SecondEvent")
                            .AddAction("Greet", act => act.SetFunctionRef("greetingFunction", "{ 'name' : '$.name' }").SetActionDataFilter(string.Empty, "$.firstEvent", "$.result"))
                            .AddAction("Greet", act => act.SetFunctionRef("greetingFunction", "{ 'name' : '$.name' }").SetActionDataFilter(string.Empty, "$.secondEvent", "$.result"))
                            .Build()
                    )
                    .End()
                )
                .Build();
            await runtimeJob.RegisterWorkflowDefinition(workflowDefinition);
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            runtimeJob.Start();
            var firstEventData = JObject.Parse("{'name': 'firstEvent'}");
            var secondEventData = JObject.Parse("{'name': 'secondEvent'}");
            await runtimeJob.Publish(new CloudEventMessage
            {
                Id = "id1",
                Source = "firstEventSource",
                Type = "firstEventType",
                SpecVersion = "1.0",
                Data = firstEventData
            });
            await runtimeJob.Publish(new CloudEventMessage
            {
                Id = "id2",
                Source = "secondEventSource",
                Type = "secondEventType",
                SpecVersion = "1.0",
                Data = secondEventData
            });
            instance = runtimeJob.WaitTerminate(instance.Id);
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"firstEvent\": \"Welcome to Serverless Workflow, firstEvent!\",\r\n  \"secondEvent\": \"Welcome to Serverless Workflow, secondEvent!\"\r\n}", instance.OutputStr);
        }

        [Fact]
        public async Task When_Send_Two_Events_With_Same_Id()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("greeting", "v1", "name", "description")
                .AddConsumedEvent("FirstEvent", "firstEventSource", "firstEventType")
                .AddConsumedEvent("SecondEvent", "secondEventSource", "secondEventType")
                .AddFunction(o => o.RestAPI("greetingFunction", "http://localhost/swagger/v1/swagger.json#greeting"))
                .StartsWith(o => o.Event()
                    .SetExclusive(true)
                    .AddOnEvent(
                        onevt => onevt
                            .AddEventRef("FirstEvent").AddEventRef("SecondEvent")
                            .AddAction("Greet", act => act.SetFunctionRef("greetingFunction", "{ 'name' : '$.name' }").SetActionDataFilter(string.Empty, "$.firstEvent", "$.result"))
                            .AddAction("Greet", act => act.SetFunctionRef("greetingFunction", "{ 'name' : '$.name' }").SetActionDataFilter(string.Empty, "$.secondEvent", "$.result"))
                            .Build()
                    )
                    .End()
                )
                .Build();
            await runtimeJob.RegisterWorkflowDefinition(workflowDefinition);
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            runtimeJob.Start();
            var firstEventData = JObject.Parse("{'name': 'firstEvent'}");
            var secondEventData = JObject.Parse("{'name': 'secondEvent'}");
            await runtimeJob.Publish(new CloudEventMessage
            {
                Id = "id1",
                Source = "firstEventSource",
                Type = "firstEventType",
                SpecVersion = "1.0",
                Data = firstEventData
            });
            Thread.Sleep(20);
            await runtimeJob.Publish(new CloudEventMessage
            {
                Id = "id1",
                Source = "firstEventSource",
                Type = "firstEventType",
                SpecVersion = "1.0",
                Data = secondEventData
            });
            Thread.Sleep(2000);
            var workflowInstance = runtimeJob.GetWorkflowInstance(instance.Id);
            Assert.Equal(1, workflowInstance.States.Count);
            Assert.Equal(1, workflowInstance.States.First().OnEvents.Count);
            Assert.Equal("{\r\n  \"firstEvent\": \"Welcome to Serverless Workflow, firstEvent!\"\r\n}", workflowInstance.States.First().OnEvents.First().DataStr);
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

            public WorkflowInstanceAggregate GetWorkflowInstance(string id)
            {
                var workflowInstanceRepository = _serviceProvider.GetService<IWorkflowInstanceRepository>();
                var workflowInstance = workflowInstanceRepository.Query().First(w => w.Id == id);
                return workflowInstance;
            }

            public WorkflowInstanceAggregate WaitTerminate(string id)
            {
                var workflowInstanceRepository = _serviceProvider.GetService<IWorkflowInstanceRepository>();
                var workflowInstance = workflowInstanceRepository.Query().First(w => w.Id == id);
                if (workflowInstance.Status == WorkflowInstanceStatus.TERMINATE)
                {
                    return workflowInstance;
                }

                Thread.Sleep(20);
                return WaitTerminate(id);
            }

            public Task Publish(CloudEventMessage msg)
            {
                return _busControl.Publish(msg, CancellationToken.None);
            }
        }
    }
}
