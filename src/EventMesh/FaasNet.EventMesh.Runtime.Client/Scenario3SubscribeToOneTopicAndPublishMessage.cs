using CloudNative.CloudEvents;
using FaasNet.EventMesh.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Client
{
    public static class Scenario3SubscribeToOneTopicAndPublishMessage
    {
        private static RuntimeClient _subRuntimeClient;
        private static RuntimeClient _pubRuntimeClient;

        public static async Task Launch(string hostName = "localhost", int port = 4000, string subClientId = "subClientId", string subTopicName = "Person.*", string pubClientId = "pubClientId", string pubTopicName = "Person.Created")
        {
            var kvpSub = await Sub(hostName, port, subClientId, subTopicName);
            Console.WriteLine("Please press enter to publish a message");
            Console.ReadLine();
            var pubSessionId = await Pub(hostName, port, pubClientId, pubTopicName);
            Console.WriteLine("Please press enter to quit the application ...");
            Console.ReadLine();
            kvpSub.Key.Stop();
            await _subRuntimeClient.Disconnect(subClientId, kvpSub.Value);
            await _pubRuntimeClient.Disconnect(pubClientId, pubSessionId);
        }

        private static async Task<string> Pub(string hostName = "localhost", int port = 4000, string pubClientId = "pubClientId", string topicName = "Person.Created")
        {
            _pubRuntimeClient = new RuntimeClient(hostName, port);
            var sessionId = await CreateSession(_pubRuntimeClient, pubClientId, UserAgentPurpose.PUB);
            Console.WriteLine($"PUB session is created with id '{sessionId}'");
            var cloudEvent = new CloudEvent
            {
                Type = "com.github.pull.create",
                Source = new Uri("https://github.com/cloudevents/spec/pull"),
                Subject = "123",
                Id = "A234-1234-1234",
                Time = new DateTimeOffset(2018, 4, 5, 17, 31, 0, TimeSpan.Zero),
                DataContentType = "application/json",
                Data = "testttt",
                ["comexampleextension1"] = "value"
            };
            await _pubRuntimeClient.PublishMessage(pubClientId, sessionId, topicName, cloudEvent);
            return sessionId;
        }

        private static async Task<KeyValuePair<SubscriptionResult, string>> Sub(string hostName = "localhost", int port = 4000, string subClientId = "pubClientId", string subTopicName = "Person.*")
        {
            _subRuntimeClient = new RuntimeClient(hostName, port);
            var sessionId = await CreateSession(_subRuntimeClient, subClientId, UserAgentPurpose.SUB);
            Console.WriteLine($"SUB session is created with id '{sessionId}'");
            var subscriptionResult = await SubscribeTopic(subClientId, sessionId, subTopicName);
            return new KeyValuePair<SubscriptionResult, string>(subscriptionResult, sessionId);
        }

        private static async Task<string> CreateSession(RuntimeClient runtimeClient, string clientId, UserAgentPurpose purpose)
        {
            var helloResponse = await runtimeClient.Hello(new UserAgent
            {
                ClientId = clientId,
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                BufferCloudEvents = 1,
                Version = "0",
                Purpose = purpose,
                Vpn = "default"
            });
            return helloResponse.SessionId;
        }

        private static async Task<SubscriptionResult> SubscribeTopic(string clientId, string sessionId, string topicName)
        {
            return await _subRuntimeClient.Subscribe(clientId, sessionId, new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = topicName,
                }
            }, (msg) =>
            {
                var cloudEvts = string.Join(",", msg.CloudEvents.Select(c => c.Data));
                Console.WriteLine($"Receive '{msg.CloudEvents.Count()}' messages: {cloudEvts}, BrokerName : {msg.BrokerName}, urn : {string.Join(',', msg.BridgeServers.Select(b => b.Urn))}");
            });
        }
    }
}
