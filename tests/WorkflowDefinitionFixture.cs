using FaasNet.Runtime.Builders;
using FaasNet.Runtime.CloudEvent.Models;
using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Domains.Enums;
using FaasNet.Runtime.Domains.Instances;
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
            var workflowDefinition = WorkflowDefinitionBuilder.New("helloWorld", 1, "name", "description")
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
            var workflowDefinition = WorkflowDefinitionBuilder.New("injectAndFilterPersons", 1, "name", "description")
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
                }).End().SetOutputFilter("{ \"people\" : \"${ (.people | select(.[].age < 40)) }\" }"))
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"people\": [\r\n    {\r\n      \"fname\": \"Marry\",\r\n      \"lname\": \"Allice\",\r\n      \"address\": \"1234 SomeStreet\",\r\n      \"age\": 25\r\n    },\r\n    {\r\n      \"fname\": \"Kelly\",\r\n      \"lname\": \"Mill\",\r\n      \"address\": \"1234 SomeStreet\",\r\n      \"age\": 30\r\n    }\r\n  ]\r\n}", instance.OutputStr);
        }

        [Fact]
        public async Task When_Run_GreetingFunction()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("greeting", 1, "name", "description")
                .AddFunction(o => o.RestAPI("greetingFunction", "http://localhost/swagger/v1/swagger.json#greeting"))
                .StartsWith(o => o.Operation().SetActionMode(WorkflowDefinitionActionModes.Sequential).AddAction("Greet",
                (act) => act.SetFunctionRef("greetingFunction", "{ \"queries\" : { \"name\" : \"${ .person.name }\" } }")
                .SetActionDataFilter(string.Empty, "${ .person.message }", "${ .result }"))
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
            var workflowDefinition = WorkflowDefinitionBuilder.New("greeting", 1, "name", "description")
                .AddConsumedEvent("GreetingEvent", "greetingEventSource", "greetingEventType")
                .AddFunction(o => o.RestAPI("greetingFunction", "http://localhost/swagger/v1/swagger.json#greeting"))
                .StartsWith(o => o.Event()
                    .SetExclusive(false)
                    .AddOnEvent(
                        onevt => onevt.AddEventRef("GreetingEvent").AddAction("Greet", act => act.SetFunctionRef("greetingFunction", "{ \"queries\" : { \"name\" : \"${ .person.message }\" } }").SetActionDataFilter(string.Empty, "${ .person.message }", "${ .result }")).Build()
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
            var workflowDefinition = WorkflowDefinitionBuilder.New("greeting", 1, "name", "description")
                .AddConsumedEvent("FirstEvent", "firstEventSource", "firstEventType")
                .AddConsumedEvent("SecondEvent", "secondEventSource", "secondEventType")
                .AddFunction(o => o.RestAPI("greetingFunction", "http://localhost/swagger/v1/swagger.json#greeting"))
                .StartsWith(o => o.Event()
                    .SetExclusive(false)
                    .AddOnEvent(
                        onevt => onevt
                            .AddEventRef("FirstEvent").AddEventRef("SecondEvent")
                            .AddAction("Greet", act => act.SetFunctionRef("greetingFunction", "{ \"queries\" : { \"name\" : \"${ .name }\" } }").SetActionDataFilter(string.Empty, "${ .firstEvent }", "${ .result }"))
                            .AddAction("Greet", act => act.SetFunctionRef("greetingFunction", "{ \"queries\" : { \"name\" : \"${ .name }\" } }").SetActionDataFilter(string.Empty, "${ .secondEvent }", "${ .result }"))
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
            runtimeJob.Wait(instance.Id, i =>
            {
                var firstState = i.States.FirstOrDefault();
                if (firstState == null || !firstState.Events.Any())
                {
                    return false;
                }

                var evt = firstState.Events.FirstOrDefault(e => e.Source == "firstEventSource");
                if (evt == null)
                {
                    return false;
                }

                return evt.State == WorkflowInstanceStateEventStates.CONSUMED;
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
            Assert.Equal("{\r\n  \"name\": \"secondEvent\",\r\n  \"firstEvent\": \"Welcome to Serverless Workflow, firstEvent!\",\r\n  \"secondEvent\": \"Welcome to Serverless Workflow, secondEvent!\"\r\n}", instance.OutputStr);
        }

        [Fact]
        public async Task When_Send_Two_Events_With_Same_Id()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("greeting", 1, "name", "description")
                .AddConsumedEvent("FirstEvent", "firstEventSource", "firstEventType")
                .AddConsumedEvent("SecondEvent", "secondEventSource", "secondEventType")
                .AddFunction(o => o.RestAPI("greetingFunction", "http://localhost/swagger/v1/swagger.json#greeting"))
                .StartsWith(o => o.Event()
                    .SetExclusive(true)
                    .AddOnEvent(
                        onevt => onevt
                            .AddEventRef("FirstEvent").AddEventRef("SecondEvent")
                            .AddAction("Greet", act => act.SetFunctionRef("greetingFunction", "{ \"queries\" : { \"name\" :  \"${ .name }\" } }").SetActionDataFilter(string.Empty, "${ .firstEvent }", "${ .result }"))
                            .AddAction("Greet", act => act.SetFunctionRef("greetingFunction", "{ \"queries\" : { \"name\" :  \"${ .name }\" } }").SetActionDataFilter(string.Empty, "${ .secondEvent }", "${ .result }"))
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
            runtimeJob.Wait(instance.Id, i =>
            {
                var firstState = i.States.FirstOrDefault();
                if (firstState == null || !firstState.Events.Any())
                {
                    return false;
                }

                var evt = firstState.Events.FirstOrDefault(e => e.Source == "firstEventSource");
                if (evt == null)
                {
                    return false;
                }

                return evt.State == WorkflowInstanceStateEventStates.PROCESSED;
            });
            await runtimeJob.Publish(new CloudEventMessage
            {
                Id = "id1",
                Source = "firstEventSource",
                Type = "firstEventType",
                SpecVersion = "1.0",
                Data = secondEventData
            });
            Thread.Sleep(1000);
            var workflowInstance = runtimeJob.GetWorkflowInstance(instance.Id);
            Assert.Equal(1, workflowInstance.States.Count);
            Assert.Equal("{\r\n  \"name\": \"firstEvent\",\r\n  \"firstEvent\": \"Welcome to Serverless Workflow, firstEvent!\"\r\n}", workflowInstance.States.First().Events.First().OutputLst.First().Data);
            Assert.Equal(WorkflowInstanceStateEventStates.PROCESSED, workflowInstance.States.First().Events.First().State);
        }

        [Fact]
        public async Task When_Run_Switch_Data_Condition()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("greeting", 1, "name", "description")
                .StartsWith(o => o.Switch()
                    .AddDataCondition("MoreThan18", "StartApplication", "${ .applicant.age >= 18 }")
                    .AddDataCondition("LessThan18", "RejectApplication", "${ .applicant.age < 18 }")
                )
                .Then(o => o.Inject().Data(new { reason = "accepted" }).End().SetName("StartApplication"))
                .Then(o => o.Inject().Data(new { reason = "rejected" }).End().SetName("RejectApplication"))
                .Build();
            await runtimeJob.RegisterWorkflowDefinition(workflowDefinition);
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{ 'applicant' : { 'age' : 18 }}");
            var secondInstance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{ 'applicant' : { 'age' : 15 }}");
            Assert.Equal(3, instance.States.Count);
            Assert.Equal(3, secondInstance.States.Count);
            Assert.Equal("{\r\n  \"reason\": \"accepted\"\r\n}", instance.OutputStr);
            Assert.Equal("{\r\n  \"reason\": \"rejected\"\r\n}", secondInstance.OutputStr);
        }

        [Fact]
        public async Task When_Run_Switch_Event_Condition()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("greeting", 1, "name", "description")
                .AddConsumedEvent("visaApprovedEvt", "visaCheckSource", "VisaApproved")
                .AddConsumedEvent("visaRejectedEvt", "visaCheckSource", "VisaRejected")
                .StartsWith(o => o.Switch()
                    .AddEventCondition("visaApproved", "visaApprovedEvt", "HandleApprovedVisa")
                    .AddEventCondition("visaRejected", "visaRejectedEvt", "HandleRejectedVisa")
                )
                .Then(o => o.Inject().Data(new { reason = "accepted" }).End().SetName("HandleApprovedVisa"))
                .Then(o => o.Inject().Data(new { reason = "rejected" }).End().SetName("HandleRejectedVisa"))
                .Build();
            await runtimeJob.RegisterWorkflowDefinition(workflowDefinition);
            runtimeJob.Start();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            var firstEventData = JObject.Parse("{'name': 'firstEvent'}");
            await runtimeJob.Publish(new CloudEventMessage
            {
                Id = "id1",
                Source = "visaCheckSource",
                Type = "VisaApproved",
                SpecVersion = "1.0",
                Data = firstEventData
            });
            instance = runtimeJob.WaitTerminate(instance.Id);
            var secondInstance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            var secondEventData = JObject.Parse("{'name': 'secondEvent'}");
            await runtimeJob.Publish(new CloudEventMessage
            {
                Id = "id2",
                Source = "visaCheckSource",
                Type = "VisaRejected",
                SpecVersion = "1.0",
                Data = firstEventData
            });
            secondInstance = runtimeJob.WaitTerminate(secondInstance.Id);
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, secondInstance.Status);
            Assert.Equal("{\r\n  \"reason\": \"accepted\"\r\n}", instance.OutputStr);
            Assert.Equal("{\r\n  \"reason\": \"rejected\"\r\n}", secondInstance.OutputStr);
        }

        [Fact]
        public async Task When_Run_ForeachState()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("solvemathproblems", 1, "name", "description")
                .AddFunction(o => o.RestAPI("solveMathExpressionFunction", "http://localhost/swagger/v1/swagger.json#calculator"))
                .StartsWith(o => o.Foreach()
                    .SetInputCollection("${ .expressions }")
                    .SetOutputCollection("${ .results }")
                    .SetMode(WorkflowDefinitionForeachStateModes.Sequential)
                    .AddAction("", (act) =>
                    {
                        act.SetFunctionRef("solveMathExpressionFunction", null);
                    })
                    .End()
                )
                .Build();
            await runtimeJob.RegisterWorkflowDefinition(workflowDefinition);
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{ 'expressions' : ['2+2', '4-1', '10x3']}");
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("[\r\n  4,\r\n  3,\r\n  30\r\n]", instance.OutputStr);
        }

        [Fact]
        public async Task When_Run_CallbackState()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("creditCheckCompleteType", 1, "name", "description")
                .AddFunction(o => o.RestAPI("creditCheckFunction", "http://localhost/swagger/v1/swagger.json#creditCheck"))
                .AddConsumedEvent("CreditCheckCompletedEvent", "creditCheckSource", "creditCheckCompleteType")
                .StartsWith(o => o.Callback()
                    .SetAction("callCreditCheckMicroservice", act => act.SetFunctionRef("creditCheckFunction", string.Empty))
                    .SetEventRef("CreditCheckCompletedEvent")
                    .End()
                )
                .Build();
            await runtimeJob.RegisterWorkflowDefinition(workflowDefinition);
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            runtimeJob.Start();
            var evtData = JObject.Parse("{'name': 'firstEvent'}");
            await runtimeJob.Publish(new CloudEventMessage
            {
                Id = "id1",
                Source = "creditCheckSource",
                Type = "creditCheckCompleteType",
                SpecVersion = "1.0",
                Data = evtData
            });
            instance = runtimeJob.WaitTerminate(instance.Id);
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"name\": \"firstEvent\"\r\n}", instance.OutputStr);
        }

        [Fact]
        public async Task When_Run_SendEmailWithHttpPost()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("sendcustomemail", 1, "name", "description")
                .AddFunction(o => o.RestAPI("sendEmailFunction", "http://localhost/swagger/v1/swagger.json#sendEmailPost"))
                .StartsWith(o => o.Operation().SetActionMode(WorkflowDefinitionActionModes.Sequential).AddAction("SendEmail",
                (act) => act.SetFunctionRef("sendEmailFunction", "{ \"properties\" : { \"address\" : \"${ .person.email }\", \"body\" : \"${ .message }\", \"parameter\" : { \"name\" : \"${ .name }\" }, \"destinations\" : [\"destination1\"], \"pictures\" : [ { \"url\" : \"url1\" } ] } }")
                .SetActionDataFilter(string.Empty, "${ .emailResult }", string.Empty))
                .End())
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{'person' : { 'email': 'agentsimpleidserver@gmail.com' }, 'message': 'Hello', 'name': 'Name' }");
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
        }

        [Fact]
        public async Task When_Run_SendEmailWithHttpPostAndParameterIsMissing()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("sendcustomemail", 1, "name", "description")
                .AddFunction(o => o.RestAPI("sendEmailFunction", "http://localhost/swagger/v1/swagger.json#sendEmailPost"))
                .StartsWith(o => o.Operation().SetActionMode(WorkflowDefinitionActionModes.Sequential).AddAction("SendEmail",
                (act) => act.SetFunctionRef("sendEmailFunction", "{ \"properties\" : { \"address\" : \"${ .person.email }\", \"body\" : \"${ .message }\", parameter : { } } }")
                .SetActionDataFilter(string.Empty, string.Empty, ".emailResult"))
                .End())
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{'person' : { 'email': 'agentsimpleidserver@gmail.com' }, 'message': 'Hello', 'name': 'Name' }");
            var state = instance.States.First();
            Assert.Equal(WorkflowInstanceStatus.ACTIVE, instance.Status);
            Assert.Equal(WorkflowInstanceStateStatus.ERROR, state.Status);
            Assert.Equal(2, state.Histories.Count);
        }

        [Fact]
        public async Task When_Run_SendEmailWithHttpGet()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("sendEmailGet", 1, "name", "description")
                .AddFunction(o => o.RestAPI("sendEmailFunction", "http://localhost/swagger/v1/swagger.json#sendEmailGet"))
                .StartsWith(o => o.Operation().SetActionMode(WorkflowDefinitionActionModes.Sequential).AddAction("SendEmail",
                (act) => act.SetFunctionRef("sendEmailFunction", "{ \"queries\" : { \"Address\" : \"${ .person.email }\", \"Body\" : \"${ .message }\" } }")
                .SetActionDataFilter(string.Empty, ".emailResult", string.Empty))
                .End())
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{'person' : { 'email': 'agentsimpleidserver@gmail.com' }, 'message': 'Hello', 'name': 'Name' }");
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
        }

        [Fact]
        public async Task When_Publish_Event()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = WorkflowDefinitionBuilder.New("publishEvent", 1, "name", "description")
                .AddFunction(o => o.AsyncAPI("publishEvent", "http://localhost/asyncapi/asyncapi.json#PublishLightMeasuredEvent"))
                .StartsWith(o => o.Operation().SetActionMode(WorkflowDefinitionActionModes.Sequential).AddAction("publishEvent",
                (act) => act.SetFunctionRef("publishEvent", "{ \"id\" : 1, \"lumens\" : 3 } ")
                .SetActionDataFilter(string.Empty, ".emailResult", string.Empty))
                .End())
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{ }", new Dictionary<string, string> { { "userName", "guest" }, { "password", "guest" } });
            Assert.Equal(WorkflowInstanceStatus.TERMINATE, instance.Status);
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

            public Task<WorkflowInstanceAggregate> InstanciateAndLaunch(WorkflowDefinitionAggregate workflowDef, string input, Dictionary<string, string> parameters)
            {
                var runtimeEngine = _serviceProvider.GetRequiredService<IRuntimeEngine>();
                return runtimeEngine.InstanciateAndLaunch(workflowDef, input, parameters, CancellationToken.None);
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

            public WorkflowInstanceAggregate Wait(string id, Func<WorkflowInstanceAggregate, bool> callback)
            {
                var workflowInstanceRepository = _serviceProvider.GetService<IWorkflowInstanceRepository>();
                var workflowInstance = workflowInstanceRepository.Query().First(w => w.Id == id);
                if (callback(workflowInstance))
                {
                    return workflowInstance;
                }

                Thread.Sleep(20);
                return Wait(id, callback);
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
