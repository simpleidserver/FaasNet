using EventMesh.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Client
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            await LaunchEventMeshClient();
            Console.WriteLine("Please press enter to quit the application ...");
            Console.ReadLine();
            return 1;
        }

        private static async Task LaunchEventMeshClient()
        {
            Console.WriteLine("Subscribe to topic 'Test.Coucou'");
            var client = new RuntimeClient();
            // Create a session.
            await client.Hello(new UserAgent
            {
                ClientId = "7127b7d9-a4b3-4728-b8d6-7c573503be98",
                Environment = "TST",
                Username = "userName",
                Password = "password",
                Pid = 2000,
                Purpose = UserAgentPurpose.SUB,
                Version = "0",
                BufferCloudEvents = 2
            });
            // Subscribe to a topic.
            await client.Subscribe(new List<SubscriptionItem>
            {
                new SubscriptionItem
                {
                    Topic = "Test.COUCOU",
                }
            }, (msg) =>
            {
                var cloudEvts = string.Join(",", msg.CloudEvents.Select(c => c.Data));
                Console.WriteLine($"Receive '{msg.CloudEvents.Count()}' messages: {cloudEvts}");
            });
        }
    }
}
