using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace FaasNet.RaftConsensus.Service
{
    internal partial class Program
    {
        public static void LaunchPeers()
        {
            LaunchRaftConsensusPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5002) }, false, "node1", 5001);
            LaunchRaftConsensusPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5001) }, false, "node2", 5002);

            Console.WriteLine("Press any key to add an entry");
            Console.ReadLine();
            AddLogEntry(5002, false);

            Console.WriteLine("Press any key to add an entry");
            Console.ReadLine();
            AddLogEntry(5002, false);

            Console.WriteLine("Press any key to display state of Peer 5001");
            Console.ReadLine();
            DisplayPeerState(5001, false);

            Console.WriteLine("Press any key to display state of Peer 5002");
            Console.ReadLine();
            DisplayPeerState(5002, false);

            Console.WriteLine("Press any key to get logs of Peer 5001");
            Console.ReadLine();
            DisplayLogs(5001, false);

            Console.WriteLine("Press any key to get logs of Peer 5002");
            Console.ReadLine();
            DisplayLogs(5002, false);
        }

        private static IPeerHost LaunchRaftConsensusPeer(ConcurrentBag<ClusterPeer> clusterPeers, bool isTcp = false, string nodeName = "node1", int port = 5001)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var peerHostFactory = PeerHostFactory.NewUnstructured(o => {
                o.Port = port;
            }, clusterPeers, s =>
            {
                s.AddLogging(l =>
                {
                    l.AddConsole();
                });
            })
                .AddRaftConsensus(o =>
                {
                    o.ConfigurationDirectoryPath = Path.Combine(path, nodeName);
                    o.LeaderCallback = () =>
                    {
                        Console.WriteLine("There a is a leader");
                    };
                })
                .UseRocksDB();
            if (isTcp) peerHostFactory.UseTCPTransport();
            else peerHostFactory.UseUDPTransport();
            var peerHost = peerHostFactory.Build();
            peerHost.Start();
            return peerHost;
        }

        private static async void AddLogEntry(int port, bool isTcp = false)
        {
            using (var client = PeerClientFactory.Build<RaftConsensusClient>("localhost", port, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
                await client.AppendEntry(Encoding.UTF8.GetBytes("value"), 5000);
        }

        private static async void DisplayPeerState(int port, bool isTcp = false)
        {
            using (var client = PeerClientFactory.Build<RaftConsensusClient>("localhost", port, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                var peerState = (await client.GetPeerState(5000)).First();
                Console.WriteLine($"Status {peerState.Status}");
                Console.WriteLine($"Term {peerState.Term}");
                Console.WriteLine($"CommitIndex {peerState.CommitIndex}");
                Console.WriteLine($"LastApplied {peerState.LastApplied}");
                Console.WriteLine($"VotedFor {peerState.VotedFor}");
            }
        }

        private static async void DisplayLogs(int port, bool isTcp = false)
        {
            using (var client = PeerClientFactory.Build<RaftConsensusClient>("localhost", port, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                var result = (await client.GetLogs(1, 5000)).First();
                foreach (var log in result.Entries)
                {
                    Console.WriteLine($"Index {log.Index}");
                    Console.WriteLine($"Term {log.Term}");
                }

                Console.WriteLine();
            }
        }
    }
}
