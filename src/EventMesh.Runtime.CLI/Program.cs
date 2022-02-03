using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;

namespace EventMesh.Runtime.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            LaunchMQTT();
            Console.WriteLine("Please press Enter to quit the application...");
            Console.ReadLine();
        }

        private static void LaunchMQTT()
        {
            Console.WriteLine("Launch EventMesh runtime...");
            var serviceProvider = new RuntimeHostBuilder().AddMQTT().ServiceCollection.BuildServiceProvider();
            var consumer = serviceProvider.GetServices<IMessageConsumer>().First();
            consumer.Start(CancellationToken.None).Wait();
            consumer.Subscribe("Test.*", CancellationToken.None).Wait();
            Console.WriteLine("EventMesh runtime is launched !");
        }
    }
}
