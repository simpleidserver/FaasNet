using CloudNative.CloudEvents;
using FaasNet.Common;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Runtime;
using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventStore;
using FaasNet.StateMachine.Runtime.AsyncAPI.Channels.Amqp;
using FaasNet.StateMachine.Runtime.Builders;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Factories;
using FaasNet.StateMachine.Runtime.Serializer;
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
            var runtimeJob = new StateMachineJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("helloWorld", 1, "name", "description", "default")
                .StartsWith(o => o.Inject().Data(new { result = "Hello World!" }).End())
                .Build();
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("Hello World!", instance.GetOutput()["result"].ToString());
        }

        [Fact]
        public async Task When_Inject_Persons_And_Filter()
        {
            var runtimeJob = new StateMachineJob();
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
            var stateMachineJob = new StateMachineJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("greeting", 1, "name", "description", "default", "rootTopic")
                .AddConsumedEvent("FirstEvent", "https://github.com/cloudevents/spec/pull", "firstEventType", "firstCloudEvt")
                .AddConsumedEvent("SecondEvent", "https://github.com/cloudevents/spec/pull", "secondEventType", "secondCloudEvt")
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
            await stateMachineJob.RegisterWorkflowDefinition(workflowDefinition);
            var instance = await stateMachineJob.InstanciateAndLaunch(workflowDefinition, "{}");
            stateMachineJob.Start();
            await stateMachineJob.Publish("rootTopic/firstCloudEvt", new CloudEvent
            {
                Type = "firstEventType",
                Source = new Uri("https://github.com/cloudevents/spec/pull"),
                Subject = "123",
                Id = "A234-1234-1234",
                Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                DataContentType = "application/json",
                Data = "{'name': 'firstEvent'}"
            });
            stateMachineJob.Wait(instance.Id, i =>
            {
                var firstState = i.States.FirstOrDefault();
                if (firstState == null || !firstState.Events.Any())
                {
                    return false;
                }

                var evt = firstState.Events.FirstOrDefault(e => e.Type == "firstEventType");
                if (evt == null)
                {
                    return false;
                }

                return evt.State == StateMachineInstanceStateEventStates.CONSUMED;
            });
            await stateMachineJob.Publish("rootTopic/secondCloudEvt", new CloudEvent
            {
                Type = "secondEventType",
                Source = new Uri("https://github.com/cloudevents/spec/pull"),
                Subject = "123",
                Id = "A234-1234-1234",
                Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                DataContentType = "application/json",
                Data = "{'name': 'secondEvent'}"
            });
            instance = stateMachineJob.WaitTerminate(instance.Id);
            stateMachineJob.Stop();
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"name\": \"secondEvent\",\r\n  \"firstEvent\": \"Welcome to Serverless Workflow, firstEvent!\",\r\n  \"secondEvent\": \"Welcome to Serverless Workflow, secondEvent!\"\r\n}", instance.OutputStr);
        }

        [Fact]
        public async Task When_SameEvent_Is_Sent_Twice_Then_OnlyFirstEventIsConsumed()
        {
            var runtimeJob = new StateMachineJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("greeting", 1, "name", "description", "default", "rootTopic")
                .AddConsumedEvent("FirstEvent", "https://github.com/cloudevents/spec/pull", "firstevent", "firstevent")
                .AddConsumedEvent("SecondEvent", "https://github.com/cloudevents/spec/pull", "secondevent", "secondevent")
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
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            runtimeJob.Start();
            await runtimeJob.Publish("rootTopic/firstevent", new CloudEvent
            {
                Type = "firstevent",
                Source = new Uri("https://github.com/cloudevents/spec/pull"),
                Subject = "123",
                Id = "A234-1234-1234",
                Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                DataContentType = "application/json",
                Data = "{'name': 'firstEvent'}"
            });
            runtimeJob.Wait(instance.Id, i =>
            {
                var firstState = i.States.FirstOrDefault();
                if (firstState == null || !firstState.Events.Any())
                {
                    return false;
                }

                var evt = firstState.Events.FirstOrDefault(e => e.Type == "firstevent");
                if (evt == null)
                {
                    return false;
                }

                return evt.State == StateMachineInstanceStateEventStates.PROCESSED;
            });
            await runtimeJob.Publish("rootTopic/firstevent", new CloudEvent
            {
                Type = "firstevent",
                Source = new Uri("https://github.com/cloudevents/spec/pull"),
                Subject = "123",
                Id = "A234-1234-1234",
                Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                DataContentType = "application/json",
                Data = "{'name': 'secondEvent'}"
            });
            Thread.Sleep(2000);
            runtimeJob.Stop();
            var workflowInstance = runtimeJob.GetWorkflowInstance(instance.Id);
            Assert.Equal(1, workflowInstance.States.Count);
            Assert.Equal("{\r\n  \"name\": \"firstEvent\",\r\n  \"firstEvent\": \"Welcome to Serverless Workflow, firstEvent!\"\r\n}", workflowInstance.States.First().Events.First().OutputData);
            Assert.Equal(StateMachineInstanceStateEventStates.PROCESSED, workflowInstance.States.First().Events.First().State);
        }

        [Fact]
        public async Task When_Call_GreetingFunction_With_Event()
        {
            var serializer = new RuntimeSerializer();
            var runtimeJob = new StateMachineJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("greeting", 1, "name", "description", "default", "greetingTopic")
                .AddConsumedEvent("greetingEvt", "https://github.com/cloudevents/spec/pull", "com.github.pull.create", "greetingTopic")
                .AddFunction(o => o.RestAPI("greetingFunction", "http://localhost/swagger/v1/swagger.json#greeting"))
                .StartsWith(o => o.Event()
                    .SetExclusive(false)
                    .AddOnEvent(
                        onevt => onevt.AddEventRef("greetingEvt")
                            .AddAction("Greet", act => act.SetFunctionRef("greetingFunction", "{ \"queries\" : { \"name\" : \"${ .person.message }\" } }")
                            .SetActionDataFilter(string.Empty, "${ .person.message }", "${ .result }")).Build()
                    ).End()
                )
                .Build();
            await runtimeJob.RegisterWorkflowDefinition(workflowDefinition);
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            runtimeJob.Start();
            var cloudEvent = new CloudEvent
            {
                Type = "com.github.pull.create",
                Source = new Uri("https://github.com/cloudevents/spec/pull"),
                Subject = "123",
                Id = "A234-1234-1234",
                Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                DataContentType = "application/json",
                Data = "{'person': {'message': 'simpleidserver'}}"
            };
            await runtimeJob.Publish("greetingTopic", cloudEvent);
            instance = runtimeJob.WaitTerminate(instance.Id);
            runtimeJob.Stop();
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"person\": {\r\n    \"message\": \"Welcome to Serverless Workflow, simpleidserver!\"\r\n  }\r\n}", instance.OutputStr);
        }

        #endregion

        #region Switch

        [Fact]
        public async Task When_Run_Switch_Data_Condition()
        {
            var runtimeJob = new StateMachineJob();
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
            var runtimeJob = new StateMachineJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("greeting", 1, "name", "description", "default", "visa")
                .AddConsumedEvent("visaApprovedEvt", "https://github.com/cloudevents/spec/pull", "VisaApproved", "visa")
                .AddConsumedEvent("visaRejectedEvt", "https://github.com/cloudevents/spec/pull", "VisaRejected", "visa")
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
            await runtimeJob.Publish("visa", new CloudEvent
            {
                Type = "VisaApproved",
                Source = new Uri("https://github.com/cloudevents/spec/pull"),
                Subject = "123",
                Id = "A234-1234-1234",
                Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                DataContentType = "application/json",
                Data = "{'name': 'firstEvent'}"
            });
            instance = runtimeJob.WaitTerminate(instance.Id);
            var secondInstance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            var secondEventData = JObject.Parse("{'name': 'secondEvent'}");
            await runtimeJob.Publish("visa", new CloudEvent
            {
                Type = "VisaRejected",
                Source = new Uri("https://github.com/cloudevents/spec/pull"),
                Subject = "123",
                Id = "A234-1234-1234",
                Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                DataContentType = "application/json",
                Data = "{'name': 'firstEvent'}"
            });
            secondInstance = runtimeJob.WaitTerminate(secondInstance.Id);
            runtimeJob.Stop();
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
            var runtimeJob = new StateMachineJob();
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
            var runtimeJob = new StateMachineJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("creditCheckCompleteType", 1, "name", "description", "default", "credit")
                .AddFunction(o => o.RestAPI("creditCheckFunction", "http://localhost/swagger/v1/swagger.json#creditCheck"))
                .AddConsumedEvent("CreditCheckCompletedEvent", "https://github.com/cloudevents/spec/pull", "creditCheckCompleteType", "credit")
                .StartsWith(o => o.Callback()
                    .SetAction("callCreditCheckMicroservice", act => act.SetFunctionRef("creditCheckFunction", string.Empty))
                    .SetEventRef("CreditCheckCompletedEvent")
                    .End()
                )
                .Build();
            await runtimeJob.RegisterWorkflowDefinition(workflowDefinition);
            var instance = await runtimeJob.InstanciateAndLaunch(workflowDefinition, "{}");
            runtimeJob.Start();
            await runtimeJob.Publish("credit", new CloudEvent
            {
                Type = "creditCheckCompleteType",
                Source = new Uri("https://github.com/cloudevents/spec/pull"),
                Subject = "123",
                Id = "A234-1234-1234",
                Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                DataContentType = "application/json",
                Data = "{'name': 'firstEvent'}"
            });
            instance = runtimeJob.WaitTerminate(instance.Id);
            runtimeJob.Stop();
            Assert.Equal(StateMachineInstanceStatus.TERMINATE, instance.Status);
            Assert.Equal("{\r\n  \"name\": \"firstEvent\"\r\n}", instance.OutputStr);
        }

        #endregion

        #region Operation

        #region OPENAPI

        [Fact]
        public async Task When_Run_GreetingFunction()
        {
            var runtimeJob = new StateMachineJob();
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
            var runtimeJob = new StateMachineJob();
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
            var runtimeJob = new StateMachineJob();
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
            var runtimeJob = new StateMachineJob();
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
            const string json = "{ \"Id\" : 1, \"Lumens\" : 3 }";
            var payload = Encoding.UTF8.GetBytes(JToken.Parse(json).ToString());
            var runtimeJob = new StateMachineJob();
            var workflowDefinition = StateMachineDefinitionBuilder.New("publishEvent", 1, "name", "description", "default")
                .AddFunction(o => o.AsyncAPI("publishEvent", "http://localhost/asyncapi#PublishLightMeasuredEvent"))
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

        public class StateMachineJob
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly Mock<IAmqpChannelClientFactory> _ampqChannelClientFactory;
            private readonly EventConsumerHostedService _eventConsumerHostedService;
            private IRuntimeHost _runtimeHost;

            public StateMachineJob()
            {
                var factory = new CustomWebApplicationFactory<FakeStartup>();
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddStateMachineWorker()
                    .UseEventMesh();
                var mcq = new Mock<IHttpClientFactory>();
                mcq.Setup(c => c.Build()).Returns(factory.CreateClient());
                _ampqChannelClientFactory = new Mock<IAmqpChannelClientFactory>();
                serviceCollection.Remove(serviceCollection.First(s => s.ServiceType == typeof(IAmqpChannelClientFactory)));
                serviceCollection.AddSingleton(mcq.Object);
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
                var builder = new RuntimeHostBuilder(opt =>
                {
                    opt.Port = 4889;
                });
                var vpn = Vpn.Create("default", "default");
                var newClient = Client.Create(vpn.Name, "stateMachineClientId", "urn", new List<EventMesh.Client.Messages.UserAgentPurpose> { EventMesh.Client.Messages.UserAgentPurpose.SUB, EventMesh.Client.Messages.UserAgentPurpose.PUB });
                var externalClient = Client.Create(vpn.Name, "externalClient", "urn", new List<EventMesh.Client.Messages.UserAgentPurpose> { EventMesh.Client.Messages.UserAgentPurpose.PUB });
                _runtimeHost = builder.AddVpns(new List<Vpn> { vpn }).AddClients(new List<Client> { newClient, externalClient }).AddInMemoryMessageBroker().Build();
                _runtimeHost.Run();
                _eventConsumerHostedService.StartAsync(CancellationToken.None).Wait();
            }

            public void Stop()
            {
                try
                {
                    _runtimeHost.Stop();
                    _eventConsumerHostedService.StopAsync(CancellationToken.None);
                }
                catch { }
            }

            public StateMachineInstanceAggregate GetWorkflowInstance(string id)
            {
                var commitAggregateHelper = _serviceProvider.GetService<ICommitAggregateHelper>();
                return commitAggregateHelper.Get<StateMachineInstanceAggregate>(id, CancellationToken.None).Result;
            }

            public StateMachineInstanceAggregate Wait(string id, Func<StateMachineInstanceAggregate, bool> callback)
            {
                var commitAggregateHelper = _serviceProvider.GetService<ICommitAggregateHelper>();
                var stateMachineInstance = commitAggregateHelper.Get<StateMachineInstanceAggregate>(id, CancellationToken.None).Result;
                if (callback(stateMachineInstance))
                {
                    return stateMachineInstance;
                }

                Thread.Sleep(20);
                return Wait(id, callback);
            }

            public StateMachineInstanceAggregate WaitTerminate(string id)
            {
                var commitAggregateHelper = _serviceProvider.GetService<ICommitAggregateHelper>();
                var stateMachineInstance = commitAggregateHelper.Get<StateMachineInstanceAggregate>(id, CancellationToken.None).Result;
                if (stateMachineInstance.Status == StateMachineInstanceStatus.TERMINATE)
                {
                    return stateMachineInstance;
                }

                Thread.Sleep(20);
                return WaitTerminate(id);
            }

            public async Task Publish(string topic, CloudEvent msg)
            {
                using (var evtMeshClient = new EventMeshClient("externalClient", "password", "default", "localhost", 4889, 1))
                {
                    await evtMeshClient.Publish(topic, msg);
                }
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
