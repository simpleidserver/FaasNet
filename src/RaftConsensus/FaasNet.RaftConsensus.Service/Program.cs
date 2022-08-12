using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;

namespace FaasNet.RaftConsensus.Service
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            // Ajouter une nouvelle partition sur N noeuds.
            // Stocker la clef de partition sur les N noeuds.
            // Stocker les données
            // Rediriger vers les N noeuds.
            // Il faut développer un proxy qui va rediriger la requête.
            // Dans notre situation il faut une gateway.
            var firstPeer = LaunchRaftConsensusPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5002) }, "node1", 5001);
            var secondPeer = LaunchRaftConsensusPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5001) }, "node2", 5002);
            /*
            Console.WriteLine("Press any key to add an entry");
            Console.ReadLine();
            AddLogEntry(5002);
            var firstPeerPartition = WaitLogEntry("localhost", 5001, "partition");
            var secondPeerPartition = WaitLogEntry("localhost", 5002, "partition");
            Console.WriteLine($"Peer 5001 has the log {firstPeerPartition.Value}");
            Console.WriteLine($"Peer 5002 has the log {secondPeerPartition.Value}");
            Console.WriteLine("Press any key to stop the servers");
            Console.ReadLine();
            firstPeer.Stop();
            secondPeer.Stop();
            */
            Console.WriteLine("Press any key to quit the application");
            Console.ReadLine();
            return 1;
        }

        private static IPeerHost LaunchRaftConsensusPeer(ConcurrentBag<ClusterPeer> clusterPeers, string nodeName = "node1", int port = 5001)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var peerHost = PeerHostFactory.NewUnstructured(o => {
                o.Port = port;
            }, clusterPeers, (s) =>
            {
                s.AddLogging(o => o.AddConsole());
            })
                .UseUDPTransport()
                .AddRaftConsensus(o =>
                {
                    o.ConfigurationDirectoryPath = Path.Combine(path, nodeName);
                })
                .Build();
            peerHost.Start();
            return peerHost;
        }

        /*
        private static async void AddLogEntry(int port)
        {
            using (var raftConsensusClient = new UDPRaftConsensusClient("localhost", port))
            {
                await raftConsensusClient.AppendEntry("partition", "value", CancellationToken.None);
            }
        }

        private static GetEntryResult WaitLogEntry(string url, int port, string replicationId)
        {
            using (var raftConsensusClient = new UDPRaftConsensusClient(url, port))
            {
                var entry = raftConsensusClient.GetEntry(replicationId).Result;
                if (entry.IsNotFound)
                {
                    Thread.Sleep(100);
                    return WaitLogEntry(url, port, replicationId);
                }

                return entry;
            }
        }
        */
    }
}
