using FaasNet.CRDT.Client;
using FaasNet.CRDT.Core.Entities;
using FaasNet.CRDT.Core.SerializedEntities;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using System.Collections.Concurrent;

namespace FaasNet.DHT.Chord.Service
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var firstPeerHostpeerHost = LaunchCRDTPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5002) }, 5001, "peerId");
            var secondPeerHostpeerHost = LaunchCRDTPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5001) }, 5002, "peerId2");
            CheckGCounter();
            CheckGSet();
            Console.WriteLine("Press any key to stop the servers");
            Console.ReadLine();
            firstPeerHostpeerHost.Stop();
            secondPeerHostpeerHost.Stop();
            Console.WriteLine("Press any key to quit the application");
            Console.ReadLine();
            // https://github.com/cloudstateio/cloudstate/blob/16ea6f8f17c8f8b5959e626dc4e9808d9288dae6/node-support/src/crdts/pncounter.js
            // https://mwhittaker.github.io/consistency_in_distributed_systems/3_crdt.html
            return 1;
        }

        private static void CheckGCounter()
        {
            Console.WriteLine("Press any key to increment the counter 'nb_customers'");
            Console.ReadLine();
            using (var crdtClient = new UDPCRDTClient("localhost", 5001))
            {
                crdtClient.IncrementGCounter("nb_customers", 2).Wait();
            }

            Console.WriteLine("Press any key to get the counter 'nb_customers' from first Peer");
            Console.ReadLine();
            using (var crdtClient = new UDPCRDTClient("localhost", 5001))
            {
                var result = crdtClient.Get("nb_customers").Result;
                Console.WriteLine($"nb_customers = {result}");
            }

            Console.WriteLine("Press any key to get the counter 'nb_customers' from second Peer");
            Console.ReadLine();
            using (var crdtClient = new UDPCRDTClient("localhost", 5002))
            {
                var result = crdtClient.Get("nb_customers").Result;
                Console.WriteLine($"nb_customers = {result}");
            }
        }

        private static void CheckGSet()
        {
            Console.WriteLine("Press any key to add two elements");
            Console.ReadLine();
            using (var crdtClient = new UDPCRDTClient("localhost", 5001))
            {
                crdtClient.AddGSet("customers", new List<string>
                {
                    "customer1",
                    "customer2"
                }).Wait();
            }

            Console.WriteLine("Press any key to get the gset from first Peer");
            Console.ReadLine();
            using (var crdtClient = new UDPCRDTClient("localhost", 5001))
            {
                var result = crdtClient.Get("customers").Result;
                Console.WriteLine($"customers = {result}");
            }

            Console.WriteLine("Press any key to get the gset from second Peer");
            Console.ReadLine();
            using (var crdtClient = new UDPCRDTClient("localhost", 5002))
            {
                var result = crdtClient.Get("customers").Result;
                Console.WriteLine($"customers = {result}");
            }
        }

        private static IPeerHost LaunchCRDTPeer(ConcurrentBag<ClusterPeer> clusterPeers, int port = 5001, string peerId = "peerId")
        {
            var gcounter = new GCounter(peerId);
            var gset = new GSet(peerId);
            var firstSerializedEntity = new CRDTEntitySerializer().Serialize("nb_customers", gcounter);
            var secondSerializedEntity = new CRDTEntitySerializer().Serialize("customers", gset);
            var entities = new ConcurrentBag<SerializedEntity>
            {
                firstSerializedEntity,
                secondSerializedEntity
            };
            var peerHost = PeerHostFactory.New(o => {
                    o.Port = port;
                    o.PeerId = peerId;
                }, clusterPeers)
                .UseUDPTransport()
                .AddCRDTProtocol(entities)
                .Build();
            peerHost.Start();
            return peerHost;
        }
    }
}
