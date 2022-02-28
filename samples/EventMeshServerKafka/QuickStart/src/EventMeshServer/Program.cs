using EventMesh.Runtime;
using System;

namespace EventMeshServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = RunKafkaEventMeshServer();
            Console.WriteLine("Please press Enter to stop the EventMeshServer");
            Console.ReadLine();
            host.Stop();
            Console.WriteLine("The EventMeshServer is stopped...");
            Console.WriteLine("Please press Enter to quit the application");
            Console.ReadLine();
        }

        private static IRuntimeHost RunKafkaEventMeshServer()
        {
            const int port = 4000;
            const string urn = "localhost";
            var builder = new RuntimeHostBuilder(opt =>
            {
                opt.Port = port;
                opt.Urn = urn;
            }).AddKafka();
            var host = builder.Build();
            host.Run();
            Console.WriteLine($"The EventMeshServer is started on '{urn}:{port}'");
            return host;
        }
    }
}
