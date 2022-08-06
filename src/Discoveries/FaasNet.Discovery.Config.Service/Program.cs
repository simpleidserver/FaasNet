using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using System.Collections.Concurrent;

namespace FaasNet.Discovery.Config.Service
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var peerHost = LaunchPeer(new ConcurrentBag<ClusterPeer>(), 5001, "seedId");
            Console.WriteLine("Press any key to display all the Peers present in the network");
            Console.ReadLine();
            DisplayCluster();
            Console.WriteLine("Press any key to stop the servers");
            Console.ReadLine();
            peerHost.Stop();
            Console.WriteLine("Press any key to quit the application");
            Console.ReadLine();
            return 1;
        }

        private static IPeerHost LaunchPeer(ConcurrentBag<ClusterPeer> clusterPeers, int port = 5001, string peerId = "peerId")
        {
            var peerHost = PeerHostFactory.NewUnstructured(o => {
                o.Url = "localhost";
                o.Port = port;
                o.PeerId = peerId;
            }, clusterPeers)
                .UseUDPTransport()
                .UseDiscoveryConfig()
                .Build();
            peerHost.Start();
            return peerHost;
        }

        private static void DisplayCluster()
        {
            using (var gossipClient = new UDPGossipClient("localhost", 5002))
            {
                var peerInfos = gossipClient.Get().Result;
                foreach(var peerInfo in peerInfos)
                {
                    Console.WriteLine($"Url = {peerInfo.Url}, Port = {peerInfo.Port}");
                }
            }
        }
    }
}
