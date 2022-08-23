using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text;

namespace FaasNet.RaftConsensus.Service
{
    internal partial class Program
    {
        public static void LaunchNodes()
        {
            var clusterPeers = new ConcurrentBag<ClusterPeer>();
            LaunchOneNodeWithTwoPeers(5000, 3000, clusterPeers);
            LaunchOneNodeWithTwoPeers(5001, 3010, clusterPeers);

            Console.WriteLine("Press enter to add a partition in the node 5000");
            Console.ReadLine();
            AddPartition(5000, "newPartitionKey");

            Console.WriteLine("Press enter to add a partition in the node 5001");
            Console.ReadLine();
            AddPartition(5001, "newPartitionKey");

            Console.WriteLine("Press enter to display the status of all the peers hosted in the node 5000");
            Console.ReadLine();
            DisplayPeersStatus(5000);

            Console.WriteLine("Press enter to display the status of all the peers hosted in the node 5001");
            Console.ReadLine();
            DisplayPeersStatus(5001);

            Console.WriteLine("Press enter to add a log to partition 'Key2'");
            Console.ReadLine();
            AddLogEntry(5000, "Key2");

            Console.WriteLine("Get logs from a partition 'Key2'");
            Console.ReadLine();
            DisplayLogs(5000, "Key2");
        }

        private static IPeerHost LaunchOneNodeWithTwoPeers(int port, int startPeerPort, ConcurrentBag<ClusterPeer> clusterPeers)
        {
            var node = PartitionedNodeHostFactory.New(options: p =>
            {
                p.Port = port;
            }, o =>
            {
                o.StartPeerPort = startPeerPort;
                o.PeerConfiguration = (s) =>
                {
                    s.AddLogging(l =>
                    {
                        l.AddConsole();
                    });
                };
            }, clusterNodes: clusterPeers)
                .UseUDPTransport()
                .UseDirectPartitionKey(new DirectPartitionPeer { Port = startPeerPort, PartitionKey = "Key" }, new DirectPartitionPeer { Port = startPeerPort + 1, PartitionKey = "Key2" })
                .UseRaftConsensusPeer()
                .Build();
            node.Start().Wait();
            return node;
        }

        private static async void AddPartition(int port, string partitionKey)
        {
            using (var partitionClient = new UDPPartitionClient("localhost", port))
            {
                await partitionClient.AddPartition(partitionKey, CancellationToken.None);
            }
        }

        private static async void DisplayPeersStatus(int port)
        {
            using (var client = new UDPRaftConsensusClient("localhost", port) { ClientType = PartitionedPeerClientTypes.BROADCAST })
            {
                var result = await client.GetPeerState(CancellationToken.None);
                foreach (var peerState in result)
                {
                    Console.WriteLine($"Status {peerState.Status}");
                    Console.WriteLine($"Term {peerState.Term}");
                    Console.WriteLine($"CommitIndex {peerState.CommitIndex}");
                    Console.WriteLine($"LastApplied {peerState.LastApplied}");
                    Console.WriteLine($"VotedFor {peerState.VotedFor}");
                    Console.WriteLine();
                }
            }
        }

        private static async void AddLogEntry(int port, string partitionKey)
        {
            using (var partitionClient = new UDPRaftConsensusClient("localhost", port) { ClientType = PartitionedPeerClientTypes.TRANSFERED, PartitionKey = partitionKey })
            {
                await partitionClient.AppendEntry(Encoding.UTF8.GetBytes("value"), CancellationToken.None);
            }
        }

        private static async void DisplayLogs(int port, string partitionKey)
        {
            using (var raftConsensusClient = new UDPRaftConsensusClient("localhost", port) { PartitionKey = partitionKey, ClientType = PartitionedPeerClientTypes.TRANSFERED })
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
