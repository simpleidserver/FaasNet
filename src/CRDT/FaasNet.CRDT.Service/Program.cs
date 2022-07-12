using FaasNet.CRDT.Client;
using FaasNet.CRDT.Core.Entities;
using FaasNet.CRDT.Core.Stores;
using FaasNet.Peer;
using System.Collections.Concurrent;

namespace FaasNet.DHT.Chord.Service
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            // UseGCounter();
            var peerHost = LaunchCRDTPeer();
            Console.WriteLine("Increment counter");
            Console.ReadLine();
            using (var crdtClient = new UDPCRDTClient("localhost", 5001))
            {
                crdtClient.IncrementGCounter("peerId", "id", 2).Wait();
            }

            peerHost.Stop();
            Console.WriteLine("Press any key to quit the application");
            Console.ReadLine();
            // https://github.com/cloudstateio/cloudstate/blob/16ea6f8f17c8f8b5959e626dc4e9808d9288dae6/node-support/src/crdts/pncounter.js
            // https://mwhittaker.github.io/consistency_in_distributed_systems/3_crdt.html
            return 1;
        }

        private static IPeerHost LaunchCRDTPeer()
        {
            var entities = new ConcurrentBag<SerializedEntity>
            {
                new SerializedEntity
                {
                    Id = "id",
                    Type = "GCounter",
                    Value = null
                }
            };
            var peerHost = PeerHostFactory.New()
                .UseUDPTransport()
                .AddCRDTProtocol(entities)
                .Build();
            peerHost.Start();
            return peerHost;
        }

        private static void UseGCounter()
        {
            var id1 = new GCounter("id1", new Dictionary<string, long>());
            var id2 = new GCounter("id2", new Dictionary<string, long>());
            var id1FirstDelta = id1.Increment().ResetAndGetDelta();
            var id1SecondDelta = id1.Increment().ResetAndGetDelta();
            var id2FirstDelta = id2.Increment().ResetAndGetDelta();
            var id2SecondDelta=  id2.Increment().ResetAndGetDelta();
            id2.ApplyDelta("id1", id1FirstDelta);
            id2.ApplyDelta("id1", id1SecondDelta);
            id1.ApplyDelta("id2", id2FirstDelta);
            id1.ApplyDelta("id2", id2SecondDelta);
            Console.WriteLine($"Node 1, Increment = {id1.Value}");
            Console.WriteLine($"Node 2, Increment = {id2.Value}");
        }
    }
}
