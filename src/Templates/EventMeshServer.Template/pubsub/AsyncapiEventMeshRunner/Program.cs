using AsyncapiEventMeshClient;
using System;
using System.Threading.Tasks;

namespace AsyncapiEventMeshRunner
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var evtMeshClient = new EvtMeshClient("PubClient", "password", "default", "localhost", 4000);
            await evtMeshClient.Publish(new AsyncapiEventMeshClient.Models.AnonymousSchema_1
            {
                Id = "id",
                Name = "Name",
                Source = "urn:f9942caf-f9db-4c72-9120-3138544e84ab",
                Type = "Type"
            });
            await evtMeshClient.Disconnect();
            Console.WriteLine("Hello World!");
        }
    }
}
