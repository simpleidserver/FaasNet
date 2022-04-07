using FaasNet.StateMachine.Runtime.AsyncAPI.Channels.Amqp;
using FaasNet.StateMachine.Runtime.Builders;
using FaasNet.StateMachine.Runtime.CloudEvent.Models;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Factories;
using FaasNet.StateMachine.Worker;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FaasNet.StateMachine.Core.Tests
{
    public class WorkflowDefinitionFixture : IClassFixture<WebApplicationFactory<FakeStartup>>
    {
        #region Inject

        [Fact]
        public async Task When_Run_HelloWorld()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("helloWorld", 1, "name", "description", "default")
                .StartsWith(o => o.Inject().Data(new { result = "Hello World!" }).End())
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("Hello World!", instance.Output["result"].ToString());
        }

        [Fact]
        public async Task When_Inject_Persons_And_Filter()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("injectAndFilterPersons", 1, "name", "description", "default")
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
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"people\": [\r\n    {\r\n      \"fname\": \"Marry\",\r\n      \"lname\": \"Allice\",\r\n      \"address\": \"1234 SomeStreet\",\r\n      \"age\": 25\r\n    },\r\n    {\r\n      \"fname\": \"Kelly\",\r\n      \"lname\": \"Mill\",\r\n      \"address\": \"1234 SomeStreet\",\r\n      \"age\": 30\r\n    }\r\n  ]\r\n}", instance.OutputStr);
        }

        #endregion

        #region Event

        [Fact]
        public async Task When_Send_Two_Events_And_Set_Exlusive_To_False()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("greeting", 1, "name", "description", "default")
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

                return evt.State == StateMachineInstanceStateEventStates.CONSUMED;
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
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"name\": \"secondEvent\",\r\n  \"firstEvent\": \"Welcome to Serverless Workflow, firstEvent!\",\r\n  \"secondEvent\": \"Welcome to Serverless Workflow, secondEvent!\"\r\n}", instance.OutputStr);
        }

        [Fact]
        public async Task When_Send_Two_Events_With_Same_Id()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("greeting", 1, "name", "description", "default")
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

                return evt.State == StateMachineInstanceStateEventStates.PROCESSED;
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
            Assert.Equal(StateMachineInstanceStateEventStates.PROCESSED, workflowInstance.States.First().Events.First().State);
        }

        [Fact]
        public async Task When_Call_GreetingFunction_With_Event()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("greeting", 1, "name", "description", "default")
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
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"person\": {\r\n    \"message\": \"Welcome to Serverless Workflow, simpleidserver!\"\r\n  }\r\n}", instance.OutputStr);
        }

        #endregion

        #region Switch

        [Fact]
        public async Task When_Run_Switch_Data_Condition()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("greeting", 1, "name", "description", "default")
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
            var workflowDefinition = StateMachineDefinitionBuilder.New("greeting", 1, "name", "description", "default")
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
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, secondInstance.Status);
            Assert.Equal("{\r\n  \"reason\": \"accepted\"\r\n}", instance.OutputStr);
            Assert.Equal("{\r\n  \"reason\": \"rejected\"\r\n}", secondInstance.OutputStr);
        }

        #endregion

        #region Foreach

        [Fact]
        public async Task When_Run_ForeachState()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("solvemathproblems", 1, "name", "description", "default")
                .AddFunction(o => o.RestAPI("solveMathExpressionFunction", "http://localhost/swagger/v1/swagger.json#calculator"))
                .StartsWith(o => o.Foreach()
                    .SetInputCollection("${ .expressions }")
                    .SetOutputCollection("${ .results }")
                    .SetMode(StateMachineDefinitionForeachStateModes.Sequential)
                    .AddAction("", (act) =>
                    {
                        act.SetFunctionRef("solveMathExpressionFunction", null);
                    })
                    .End()
                )
                .Build();
            await runtimeJob.RegisterWorkflowDefinition(workflowDefinition);
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{ 'expressions' : ['2+2', '4-1', '10x3']}");
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("[\r\n  4,\r\n  3,\r\n  30\r\n]", instance.OutputStr);
        }

        #endregion

        #region Callback

        [Fact]
        public async Task When_Run_CallbackState()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("creditCheckCompleteType", 1, "name", "description", "default")
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
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"name\": \"firstEvent\"\r\n}", instance.OutputStr);
        }

        #endregion

        #region Operation

        #region OPENAPI

        [Fact]
        public async Task When_Run_GreetingFunction()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("greeting", 1, "name", "description", "default")
                .AddFunction(o => o.RestAPI("greetingFunction", "http://localhost/swagger/v1/swagger.json#greeting"))
                .StartsWith(o => o.Operation().SetActionMode(StateMachineDefinitionActionModes.Sequential).AddAction("Greet",
                (act) => act.SetFunctionRef("greetingFunction", "{ \"queries\" : { \"name\" : \"${ .person.name }\" } }")
                .SetActionDataFilter(string.Empty, "${ .person.message }", "${ .result }"))
                .End())
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{'person' : { 'name': 'simpleidserver', 'message': 'message' }}");
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"person\": {\r\n    \"name\": \"simpleidserver\",\r\n    \"message\": \"Welcome to Serverless Workflow, simpleidserver!\"\r\n  }\r\n}", instance.OutputStr);
        }

        [Fact]
        public async Task When_Run_SendEmailWithHttpPost()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("sendcustomemail", 1, "name", "description", "default")
                .AddFunction(o => o.RestAPI("sendEmailFunction", "http://localhost/swagger/v1/swagger.json#sendEmailPost"))
                .StartsWith(o => o.Operation().SetActionMode(StateMachineDefinitionActionModes.Sequential).AddAction("SendEmail",
                (act) => act.SetFunctionRef("sendEmailFunction", "{ \"properties\" : { \"address\" : \"${ .person.email }\", \"body\" : \"${ .message }\", \"parameter\" : { \"name\" : \"${ .name }\" }, \"destinations\" : [\"destination1\"], \"pictures\" : [ { \"url\" : \"url1\" } ] } }")
                .SetActionDataFilter(string.Empty, "${ .emailResult }", string.Empty))
                .End())
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{'person' : { 'email': 'agentsimpleidserver@gmail.com' }, 'message': 'Hello', 'name': 'Name' }");
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
        }

        [Fact]
        public async Task When_Run_SendEmailWithHttpPostAndParameterIsMissing()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("sendcustomemail", 1, "name", "description", "default")
                .AddFunction(o => o.RestAPI("sendEmailFunction", "http://localhost/swagger/v1/swagger.json#sendEmailPost"))
                .StartsWith(o => o.Operation().SetActionMode(StateMachineDefinitionActionModes.Sequential).AddAction("SendEmail",
                (act) => act.SetFunctionRef("sendEmailFunction", "{ \"properties\" : { \"address\" : \"${ .person.email }\", \"body\" : \"${ .message }\", parameter : { } } }")
                .SetActionDataFilter(string.Empty, string.Empty, "${ .emailResult }"))
                .End())
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{'person' : { 'email': 'agentsimpleidserver@gmail.com' }, 'message': 'Hello', 'name': 'Name' }");
            var state = instance.States.First();
            Assert.Equal(StateMachineInstanceStatus.ACTIVE, instance.Status);
            Assert.Equal(StateMachineInstanceStateStatus.ERROR, state.Status);
            Assert.Equal(2, state.Histories.Count);
        }

        [Fact]
        public async Task When_Run_SendEmailWithHttpGet()
        {
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("sendEmailGet", 1, "name", "description", "default")
                .AddFunction(o => o.RestAPI("sendEmailFunction", "http://localhost/swagger/v1/swagger.json#sendEmailGet"))
                .StartsWith(o => o.Operation().SetActionMode(StateMachineDefinitionActionModes.Sequential).AddAction("SendEmail",
                (act) => act.SetFunctionRef("sendEmailFunction", "{ \"queries\" : { \"Address\" : \"${ .person.email }\", \"Body\" : \"${ .message }\" } }")
                .SetActionDataFilter(string.Empty, "${ .emailResult }", string.Empty))
                .End())
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{'person' : { 'email': 'agentsimpleidserver@gmail.com' }, 'message': 'Hello', 'name': 'Name' }");
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
        }

        #endregion

        #region AsyncAPI

        [Fact]
        public async Task When_Publish_Event()
        {
            const string json = "{ \"id\" : 1, \"lumens\" : 3 }";
            var payload = Encoding.UTF8.GetBytes(JToken.Parse(json).ToString());
            var runtimeJob = new RuntimeJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("publishEvent", 1, "name", "description", "default")
                .AddFunction(o => o.AsyncAPI("publishEvent", "http://localhost/asyncapi/asyncapi.json#PublishLightMeasuredEvent"))
                .StartsWith(o => o.Operation().SetActionMode(StateMachineDefinitionActionModes.Sequential).AddAction("publishEvent",
                (act) => act.SetFunctionRef("publishEvent", json)
                .SetActionDataFilter(string.Empty, string.Empty, string.Empty))
                .End())
                .Build();
            var connectionFactory = new Mock<IConnectionFactory>();
            var connection = new Mock<IConnection>();
            var model = new Mock<IModel>();
            connectionFactory.Setup(c => c.CreateConnection()).Returns(connection.Object);
            var basicProperties = new Mock<IBasicProperties>();
            model.Setup(c => c.CreateBasicProperties()).Returns(basicProperties.Object);
            connection.Setup(c => c.CreateModel()).Returns(model.Object);
            runtimeJob.AmpqChannelClientFactory.Setup(v => v.Build(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Returns(connectionFactory.Object);
            runtimeJob.AmpqChannelClientFactory.SetupGet(s => s.SecurityType).Returns(AmqpChannelUserPasswordClientFactory.SECURITY_TYPE);
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{ }", new Dictionary<string, string> { { "userName", "guest" }, { "password", "guest" } });
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
            model.Verify(m => m.BasicPublish("testExchange", "r1", false, It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()));
        }

        #endregion

        #endregion

        public class RuntimeJob
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly Mock<IAmqpChannelClientFactory> _ampqChannelClientFactory;
            private readonly EventConsumerHostedService _eventConsumerHostedService;

            public RuntimeJob()
            {
                var factory = new CustomWebApplicationFactory<FakeStartup>();
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddStateMachineWorker();
                serviceCollection.Remove(serviceCollection.First(s => s.ServiceType == typeof(IAmqpChannelClientFactory)));
                var mcq = new Mock<IHttpClientFactory>();
                mcq.Setup(c => c.Build()).Returns(factory.CreateClient());
                serviceCollection.AddSingleton(mcq.Object);
                _ampqChannelClientFactory = new Mock<IAmqpChannelClientFactory>();
                serviceCollection.AddSingleton(_ampqChannelClientFactory.Object);
                _serviceProvider = serviceCollection.BuildServiceProvider();
                _eventConsumerHostedService = new EventConsumerHostedService(_serviceProvider);
            }

            public Mock<IAmqpChannelClientFactory> AmpqChannelClientFactory
            {
                get
                {
                    return _ampqChannelClientFactory;
                }
            }

            public Task RegisterWorkflowDefinition(StateMachineDefinitionAggregate workflowDef)
            {
                // var repository = _serviceProvider.GetRequiredService<IStateMachineDefinitionRepository>();
                // return repository.Add(workflowDef, CancellationToken.None);
                return Task.CompletedTask;
            }

            public Task<StateMachineInstanceAggregate> InstanciateAndLaunch(StateMachineDefinitionAggregate workflowDef, string input)
            {
                var runtimeEngine = _serviceProvider.GetRequiredService<IStateMachineLauncher>();
                return runtimeEngine.InstanciateAndLaunch(workflowDef, input, CancellationToken.None);
            }

            public Task<StateMachineInstanceAggregate> InstanciateAndLaunch(StateMachineDefinitionAggregate workflowDef, string input, Dictionary<string, string> parameters)
            {
                var runtimeEngine = _serviceProvider.GetRequiredService<IStateMachineLauncher>();
                return runtimeEngine.InstanciateAndLaunch(workflowDef, input, parameters, CancellationToken.None);
            }

            public void Start()
            {
                _eventConsumerHostedService.StartAsync(CancellationToken.None).Wait();
            }

            public void Stop()
            {
                _eventConsumerHostedService.StopAsync(CancellationToken.None).Wait();
            }

            public StateMachineInstanceAggregate GetWorkflowInstance(string id)
            {
                // var workflowInstanceRepository = _serviceProvider.GetService<IStateMachineInstanceRepository>();
                // var workflowInstance = workflowInstanceRepository.Query().First(w => w.Id == id);
                // return workflowInstance;
                return null;
            }

            public StateMachineInstanceAggregate Wait(string id, Func<StateMachineInstanceAggregate, bool> callback)
            {
                /*
                var workflowInstanceRepository = _serviceProvider.GetService<IStateMachineInstanceRepository>();
                var workflowInstance = workflowInstanceRepository.Query().First(w => w.Id == id);
                if (callback(workflowInstance))
                {
                    return workflowInstance;
                }
                */

                Thread.Sleep(20);
                return Wait(id, callback);
            }

            public StateMachineInstanceAggregate WaitTerminate(string id)
            {
                /*
                var workflowInstanceRepository = _serviceProvider.GetService<IStateMachineInstanceRepository>();
                var workflowInstance = workflowInstanceRepository.Query().First(w => w.Id == id);
                if (workflowInstance.Status == StateMachineInstanceStatus.TERMINATE)
                {
                    return workflowInstance;
                }
                */

                Thread.Sleep(20);
                return WaitTerminate(id);
            }

            public Task Publish(CloudEventMessage msg)
            {
                // return _busControl.Publish(msg, CancellationToken.None);
                return Task.CompletedTask;
            }
        }

        public class EventConsumerHostedService : IHostedService
        {
            private readonly IEventConsumerStore _eventConsumerStore;

            public EventConsumerHostedService(IServiceProvider serviceProvider)
            {
                var scope = serviceProvider.CreateScope();
                _eventConsumerStore = scope.ServiceProvider.GetRequiredService<IEventConsumerStore>();
            }

            public async Task StartAsync(CancellationToken cancellationToken)
            {
                await _eventConsumerStore.Init(cancellationToken);
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                _eventConsumerStore.Stop();
                return Task.CompletedTask;
            }
        }
    }
}
