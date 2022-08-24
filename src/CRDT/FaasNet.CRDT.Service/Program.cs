using FaasNet.CRDT.Client;
using FaasNet.CRDT.Core.Entities;
using FaasNet.CRDT.Core.SerializedEntities;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using System.Collections.Concurrent;

namespace FaasNet.DHT.Chord.Service
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            // https://github.com/cloudstateio/cloudstate/blob/16ea6f8f17c8f8b5959e626dc4e9808d9288dae6/node-support/src/crdts/pncounter.js
            // https://mwhittaker.github.io/consistency_in_distributed_systems/3_crdt.html
            var firstPeerHostpeerHost = LaunchCRDTPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5002) }, 5001, false);
            var secondPeerHostpeerHost = LaunchCRDTPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5001) }, 5002, false);
            CheckGCounter(false);
            CheckGSet(false);
            CheckPNCounter(false);
            Console.WriteLine("Press any key to stop the servers");
            Console.ReadLine();
            firstPeerHostpeerHost.Stop();
            secondPeerHostpeerHost.Stop();
            Console.WriteLine("Press any key to quit the application");
            Console.ReadLine();
            return 1;
        }

        private static void CheckGCounter(bool isTcp = false)
        {
            Console.WriteLine("=== GCounter ===");
            Console.WriteLine("Press any key to increment the counter 'nb_customers'");
            Console.ReadLine();
            using (var client = PeerClientFactory.Build<CRDTClient>("localhost", 5001, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                client.IncrementGCounter("nb_customers", 2).Wait();
            }

            Console.WriteLine("Press any key to get the counter 'nb_customers' from first Peer");
            Console.ReadLine();
            using (var client = PeerClientFactory.Build<CRDTClient>("localhost", 5001, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                var result = client.Get("nb_customers").Result;
                Console.WriteLine($"nb_customers = {result}");
            }

            Console.WriteLine("Press any key to get the counter 'nb_customers' from second Peer");
            Console.ReadLine();
            using (var client = PeerClientFactory.Build<CRDTClient>("localhost", 5002, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                var result = client.Get("nb_customers").Result;
                Console.WriteLine($"nb_customers = {result}");
            }

            Console.WriteLine("================");
        }

        private static void CheckGSet(bool isTcp = false)
        {
            Console.WriteLine("=== GSet ===");
            Console.WriteLine("Press any key to add two elements");
            Console.ReadLine();
            using (var client = PeerClientFactory.Build<CRDTClient>("localhost", 5001, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                client.AddGSet("customers", new List<string>
                {
                    "customer1",
                    "customer2"
                }).Wait();
            }

            Console.WriteLine("Press any key to get the gset from first Peer");
            Console.ReadLine();
            using (var client = PeerClientFactory.Build<CRDTClient>("localhost", 5001, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                var result = client.Get("customers").Result;
                Console.WriteLine($"customers = {result}");
            }

            Console.WriteLine("Press any key to get the gset from second Peer");
            Console.ReadLine();
            using (var client = PeerClientFactory.Build<CRDTClient>("localhost", 5002, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                var result = client.Get("customers").Result;
                Console.WriteLine($"customers = {result}");
            }

            Console.WriteLine("============");
        }

        private static void CheckPNCounter(bool isTcp = false)
        {
            Console.WriteLine("=== PNCounter ===");
            Console.WriteLine("Press any key to increment the counter 'nb_errors' by 5");
            Console.ReadLine();
            using (var client = PeerClientFactory.Build<CRDTClient>("localhost", 5001, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                client.IncrementPNCounter("nb_errors", 5).Wait();
            }

            Console.WriteLine("Press any key to decrement the counter 'nb_errors' by -2");
            Console.ReadLine();
            using (var client = PeerClientFactory.Build<CRDTClient>("localhost", 5001, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                client.DecrementPNCounter("nb_errors", 2).Wait();
            }

            Console.WriteLine("Press any key to get the counter 'nb_errors' from first Peer");
            Console.ReadLine();
            using (var client = PeerClientFactory.Build<CRDTClient>("localhost", 5001, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                var result = client.Get("nb_errors").Result;
                Console.WriteLine($"nb_errors = {result}");
            }

            Console.WriteLine("Press any key to get the counter 'nb_errors' from second Peer");
            Console.ReadLine();
            using (var client = PeerClientFactory.Build<CRDTClient>("localhost", 5002, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                var result = client.Get("nb_errors").Result;
                Console.WriteLine($"nb_errors = {result}");
            }

            Console.WriteLine("=================");
        }

        private static IPeerHost LaunchCRDTPeer(ConcurrentBag<ClusterPeer> clusterPeers, int port = 5001, bool isTcp = false)
        {
            var peerId = PeerId.Build("localhost", port).Serialize();
            var gcounter = new GCounter(peerId);
            var gset = new GSet(peerId);
            var pnCounter = new PNCounter(peerId);
            var firstSerializedEntity = new CRDTEntitySerializer().Serialize("nb_customers", gcounter);
            var secondSerializedEntity = new CRDTEntitySerializer().Serialize("customers", gset);
            var thirdSerializedEntity = new CRDTEntitySerializer().Serialize("nb_errors", pnCounter);
            var entities = new ConcurrentBag<SerializedEntity>
            {
                firstSerializedEntity,
                secondSerializedEntity,
                thirdSerializedEntity
            };
            var peerHostFactory = PeerHostFactory.NewUnstructured(o =>
            {
                o.Port = port;
            }, clusterPeers)
                .AddCRDTProtocol(entities);
            if (isTcp) peerHostFactory.UseTCPTransport();
            else peerHostFactory.UseUDPTransport();
            var peerHost = peerHostFactory.Build();
            peerHost.Start();
            return peerHost;
        }
    }
}
