using AsyncapiEventMeshClient;
using System;
using System.Threading.Tasks;

namespace AsyncapiEventMeshRunner
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var evtMeshClient = new EvtMeshClient("publishClient", "password", "default", "localhost", 4000);
            await evtMeshClient.Publish(new AsyncapiEventMeshClient.Models.AnonymousSchema_1
            {
                Id = "id",
                Name = "Name",
                Source = "urn:c7e8ef75-3704-487b-b999-e77f6e6b73f3",
                Type = "Type"
            });
            await evtMeshClient.Disconnect();
            Console.WriteLine("Hello World!");
        }
    }
}
