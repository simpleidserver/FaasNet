using EventMesh.Runtime;
using EventMesh.Runtime.MessageBroker;
using System;
using System.Collections.Concurrent;

namespace EventMeshServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = RunInMemoryEventMeshServer();
            Console.WriteLine("Please press Enter to stop the EventMeshServer");
            Console.ReadLine();
            host.Stop();
            Console.WriteLine("The EventMeshServer is stopped...");
            Console.WriteLine("Please press Enter to quit the application");
            Console.ReadLine();
        }

        private static IRuntimeHost RunInMemoryEventMeshServer()
        {
            const int port = 4000;
            const string urn = "localhost";
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = port;
                opt.Urn = urn;
            }).AddInMemoryMessageBroker(new ConcurrentBag<InMemoryTopic>());
            var host = builder.Build();
            host.Run();
            Console.WriteLine($"The EventMeshServer is started on '{urn}:{port}'");
            return host;
        }
    }
}
