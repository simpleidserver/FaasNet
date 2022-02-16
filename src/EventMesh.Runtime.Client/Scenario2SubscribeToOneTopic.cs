using EventMesh.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Client
{
    public static class Scenario2SubscribeToOneTopic
    {
        private static RuntimeClient _runtimeClient;

        public static async Task Launch(string hostName = "localhost", int port = 4000, string clientId = "clientId", string topicName = "Person.*")
        {
            _runtimeClient = new RuntimeClient(hostName, port);
            var sessionId = await CreateSession(clientId);
            Console.WriteLine($"session is created with id '{sessionId}'");
            var subscriptionResult = await SubscribeTopic(clientId, sessionId, topicName);
            Console.WriteLine("Please press enter to quit the application ...");
            Console.ReadLine();
            subscriptionResult.Stop();
            await _runtimeClient.Disconnect(clientId, sessionId);
        }

        private static async Task<string> CreateSession(string clientId)
        {
            var helloResponse = await _runtimeClient.Hello(new UserAgent
            {
                ClientId = clientId,
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                BufferCloudEvents = 1,
                Version = "0",
                Purpose = UserAgentPurpose.SUB
            });
            return helloResponse.SessionId;
        }

        private static async Task<SubscriptionResult> SubscribeTopic(string clientId, string sessionId, string topicName)
        {
            return await _runtimeClient.Subscribe(clientId, sessionId, new List<SubscriptionItem>
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
