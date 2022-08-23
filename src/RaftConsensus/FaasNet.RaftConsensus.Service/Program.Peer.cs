using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace FaasNet.RaftConsensus.Service
{
    internal partial class Program
    {
        public static void LaunchPeers()
        {
            LaunchRaftConsensusPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5002) }, "node1", 5001);
            LaunchRaftConsensusPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5001) }, "node2", 5002);

            Console.WriteLine("Press any key to add an entry");
            Console.ReadLine();
            AddLogEntry(5002);

            Console.WriteLine("Press any key to add an entry");
            Console.ReadLine();
            AddLogEntry(5002);

            Console.WriteLine("Press any key to display state of Peer 5001");
            Console.ReadLine();
            DisplayPeerState(5001);

            Console.WriteLine("Press any key to display state of Peer 5002");
            Console.ReadLine();
            DisplayPeerState(5002);

            Console.WriteLine("Press any key to get logs of Peer 5001");
            Console.ReadLine();
            DisplayLogs(5001);

            Console.WriteLine("Press any key to get logs of Peer 5002");
            Console.ReadLine();
            DisplayLogs(5002);
        }


        private static IPeerHost LaunchRaftConsensusPeer(ConcurrentBag<ClusterPeer> clusterPeers, string nodeName = "node1", int port = 5001)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var peerHost = PeerHostFactory.NewUnstructured(o => {
                o.Port = port;
            }, clusterPeers)
                .UseUDPTransport()
                .AddRaftConsensus(o =>
                {
                    o.ConfigurationDirectoryPath = Path.Combine(path, nodeName);
                    o.LeaderCallback = () =>
                    {
                        Console.WriteLine("There a is a leader");
                    };
                })
                .UseRocksDB()
                .Build();
            peerHost.Start();
            return peerHost;
        }

        private static async void AddLogEntry(int port)
        {
            using (var raftConsensusClient = new UDPRaftConsensusClient("localhost", port))
                await raftConsensusClient.AppendEntry(Encoding.UTF8.GetBytes("value"), CancellationToken.None);
        }

        private static async void DisplayPeerState(int port)
        {
            using (var raftConsensusClient = new UDPRaftConsensusClient("localhost", port))
            {
                var peerState = (await raftConsensusClient.GetPeerState(CancellationToken.None)).First();
                Console.WriteLine($"Status {peerState.Status}");
                Console.WriteLine($"Term {peerState.Term}");
                Console.WriteLine($"CommitIndex {peerState.CommitIndex}");
                Console.WriteLine($"LastApplied {peerState.LastApplied}");
                Console.WriteLine($"VotedFor {peerState.VotedFor}");
            }
        }

        private static async void DisplayLogs(int port)
        {
            using (var raftConsensusClient = new UDPRaftConsensusClient("localhost", port))
            {
                var result = (await raftConsensusClient.GetLogs(1, CancellationToken.None)).First();
                foreach (var log in result.Entries)
                {
                    Console.WriteLine($"Index {log.Index}");
                    Console.WriteLine($"Term {log.Term}");
                }
            }
        }
    }
}
