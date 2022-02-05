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
            while(true)
            {
                Console.WriteLine("Press enter to create a new client");
                Console.ReadLine();
                await LaunchEventMeshClient();
            }

            return 1;
        }

        private static async Task LaunchEventMeshClient()
        {
            Console.WriteLine("Subscribe to topic 'Test.Coucou'");
            var client = new RuntimeClient();
            var id = Guid.NewGuid().ToString();
            // Create a session.
            await client.Hello(new UserAgent
            {
                ClientId = id,
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
                Console.WriteLine($"Receive '{msg.CloudEvents.Count()}' message");
                string sss = "";
            });
        }
    }
}
