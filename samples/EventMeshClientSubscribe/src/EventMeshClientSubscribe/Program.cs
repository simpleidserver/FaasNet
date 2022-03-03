using FaasNet.EventMesh.Runtime;
using FaasNet.EventMesh.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventMeshClientSubscribe
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var runtimeClient = new RuntimeClient("localhost", 4000);
            var sessionId = CreateSession(runtimeClient).Result;
            var subscriptionResult = SubscribeTopic(runtimeClient, sessionId).Result;
            Console.WriteLine("Please press enter to quit the application ...");
            Console.ReadLine();
            subscriptionResult.Stop();
            runtimeClient.Disconnect("clientId", sessionId).Wait();
        }

        private static async Task<string> CreateSession(RuntimeClient runtimeClient)
        {
            var helloResponse = await runtimeClient.Hello(new UserAgent
            {
                ClientId = "clientId",
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                BufferCloudEvents = 1,
                Version = "0",
                Purpose = UserAgentPurpose.SUB
            });
            return helloResponse.SessionId;
        }

        private static async Task<SubscriptionResult> SubscribeTopic(RuntimeClient runtimeClient, string sessionId)
        {
            return await runtimeClient.Subscribe("clientId", sessionId, new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = "Person.Created",
                }
            }, (msg) =>
            {
                var cloudEvts = string.Join(",", msg.CloudEvents.Select(c => c.Data));
                Console.WriteLine($"Receive '{msg.CloudEvents.Count()}' messages: {cloudEvts}, BrokerName : {msg.BrokerName}, urn : {string.Join(',', msg.BridgeServers.Select(b => b.Urn))}");
            });
        }
    }
}
