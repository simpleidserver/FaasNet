using EventMesh.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Client
{
    public static class Scenario1SubscribeBridge
    {
        private static RuntimeClient _runtimeClient;

        public static async Task Launch()
        {
            _runtimeClient = new RuntimeClient("localhost", 4000);
            await AddBridge();
            await Subscribe();
        }

        private static async Task AddBridge()
        {
            await _runtimeClient.AddBridge("localhost", 4001);
        }

        private static async Task Subscribe()
        {
            Console.WriteLine("Subscribe to topic 'Test.Coucou'");
            // Create a session.
            await _runtimeClient.Hello(new UserAgent
            {
                ClientId = "7127b7d9-a4b3-4728-b8d6-7c573503be98",
                Environment = "TST",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0",
                BufferCloudEvents = 2
            });
            // Subscribe to a topic.
            await _runtimeClient.Subscribe(new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = "Test.COUCOU",
                }
            }, (msg) =>
            {
                var cloudEvts = string.Join(",", msg.CloudEvents.Select(c => c.Data));
                Console.WriteLine($"Receive '{msg.CloudEvents.Count()}' messages: {cloudEvts}, urn : {msg.Urn}");
            });
        }
    }
}
