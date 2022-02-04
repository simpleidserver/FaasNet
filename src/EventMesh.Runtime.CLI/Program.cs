using EventMesh.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EventMesh.Runtime.CLI
{
    class Program
    {
        private static int i = 0;
        static async Task<int> Main(string[] args)
        {
            var server = LaunchEventMeshServer();
            while(true)
            {
                Console.WriteLine("Please press enter to create a new client");
                Console.ReadLine();
                i++;
                await LaunchEventMeshClient();
            }
        }

        // private static void LaunchMQTT()
        // {
        //     Console.WriteLine("Launch EventMesh runtime...");
        //     var serviceProvider = new RuntimeHostBuilder().AddMQTT().ServiceCollection.BuildServiceProvider();
        //     var consumer = serviceProvider.GetServices<IMessageConsumer>().First();
        //     consumer.Start(CancellationToken.None).Wait();
        //     consumer.Subscribe("Test.*", CancellationToken.None).Wait();
        //     Console.WriteLine("EventMesh runtime is launched !");
        // }

        private static IRuntimeHost LaunchEventMeshServer()
        {
            Console.WriteLine("Launch EventMesh server...");
            var runtimeHost = new RuntimeHostBuilder().AddAMQP().Build();
            runtimeHost.Run();
            Console.WriteLine("EventMesh server is launched !");
            return runtimeHost;
        }

        private static async Task LaunchEventMeshClient()
        {
            Console.WriteLine("Subscribe to topic 'Test.Coucou'");
            var client = new RuntimeClient();
            client.UdpClient.Client.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, System.Net.Sockets.SocketOptionName.ReuseAddress, true);
            client.UdpClient.Client.Bind(new IPEndPoint(IPAddress.Any, 3000 + i));
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
