using EventMesh.Runtime.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace EventMesh.Runtime.Server
{
    class Program
    {
        static int Main(string[] args)
        {
            var server = LaunchEventMeshServer();
            Console.WriteLine("Press enter to quit the application");
            Console.ReadLine();
            return 1;
        }

        private static IRuntimeHost LaunchEventMeshServer()
        {
            Console.WriteLine("Launch EventMesh server...");
            var runtimeHost = new RuntimeHostBuilder().AddAMQP().Build();
            runtimeHost.Run();
            Console.WriteLine("EventMesh server is launched !");
            return runtimeHost;
        }
    }
}
