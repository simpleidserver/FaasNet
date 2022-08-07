using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace FaasNet.Discovery.Etcd.Service
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var peerHost = LaunchPeer(new ConcurrentBag<ClusterPeer>(), 5001, "seedId");
            Console.WriteLine("Press any key to display all the Peers present in the network");
            Console.ReadLine();
            DisplayCluster(peerHost.Item2);
            Console.WriteLine("Press any key to stop the servers");
            Console.ReadLine();
            peerHost.Item1.Stop();
            Console.WriteLine("Press any key to quit the application");
            Console.ReadLine();
            return 1;
        }

        private static (IPeerHost, IServiceProvider) LaunchPeer(ConcurrentBag<ClusterPeer> clusterPeers, int port = 5001, string peerId = "peerId")
        {
            var peerHostFactory = PeerHostFactory.NewUnstructured(o =>
            {
                o.Url = "localhost";
                o.Port = port;
                o.PeerId = peerId;
            }, clusterPeers)
                .UseUDPTransport()
                .UseDiscoveryEtcd();
            var peerHost = peerHostFactory.BuildWithDI();
            peerHost.Item1.Start();
            return peerHost;
        }

        private static void DisplayCluster(IServiceProvider serviceProvider)
        {
            var clusterStore = serviceProvider.GetRequiredService<IClusterStore>();
            var allNodes = clusterStore.GetAllNodes(CancellationToken.None).Result;
            foreach (var node in allNodes)
            {
                Console.WriteLine($"Url = {node.Url}, Port = {node.Port}");
            }
        }
    }
}
