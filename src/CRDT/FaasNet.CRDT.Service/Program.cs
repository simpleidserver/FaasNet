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
            Console.WriteLine("Increment counter");
            Console.ReadLine();
            using (var crdtClient = new UDPCRDTClient("localhost", 5001))
            {
                crdtClient.IncrementGCounter("nb_customers", 2).Wait();
            }

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

        private static IPeerHost LaunchCRDTPeer(ConcurrentBag<ClusterPeer> clusterPeers, int port = 5001, string peerId = "peerId")
        {
            var gcounter = new GCounter(peerId);
            var serializedEntity = new CRDTEntitySerializer().Serialize("nb_customers", gcounter);
            var entities = new ConcurrentBag<SerializedEntity>
            {
                serializedEntity
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
