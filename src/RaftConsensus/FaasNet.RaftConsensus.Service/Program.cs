using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace FaasNet.RaftConsensus.Service
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var clusterPeers = new ConcurrentBag<ClusterPeer>();
            var node = PartitionedNodeHostFactory.New(options: p =>
            {
                p.Port = 5000;
            }, clusterPeers)
                .UseUDPTransport()
                .UseDirectPartitionKey(new DirectPartitionPeer { Port = 3000, PartitionKey = "Key" }, new DirectPartitionPeer {  Port = 3001, PartitionKey = "Key2" })
                .UseRaftConsensusPeer()
                .Build();
            node.Start().Wait();
            var node2 = PartitionedNodeHostFactory.New(options: p =>
            {
                p.Port = 5001;
            }, clusterPeers)
                .UseUDPTransport()
                .UseDirectPartitionKey(new DirectPartitionPeer { Port = 3002, PartitionKey = "Key" }, new DirectPartitionPeer { Port = 3003, PartitionKey = "Key2" })
                .UseRaftConsensusPeer()
                .Build();
            node2.Start().Wait();
            /*
            var clusterPeers = new ConcurrentBag<ClusterPeer> 
            {
                new ClusterPeer("localhost", 5001) { PartitionKey = "Key" },
                new ClusterPeer("localhost", 5002) { PartitionKey = "Key" },
                new ClusterPeer("localhost", 5003) { PartitionKey = "Key2" },
                new ClusterPeer("localhost", 5004) { PartitionKey = "Key2" }
            };
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var peerHost = PeerHostFactory.NewUnstructured(o => {
                o.Port = 5001;
            }, clusterPeers)
                   .UseUDPTransport()
                   .AddRaftConsensus(o =>
                   {
                       o.ConfigurationDirectoryPath = Path.Combine(path, "node1");
                       o.LeaderCallback = () =>
                       {
                           Console.WriteLine("There a is a leader");
                       };
                   })
                   .Build();
            */
            // Ajouter un nouveau Peer avec une clef de partition.
            // Le noeud va posséder un ou plusieurs Peer.
            // Chaque peer se situe sur une partition.
            // Un peer écoute le port [30000, 3999]
            // Un peer va publier le message sur tous les noeuds d'un cluster => le cluster se trouvant sur une partition.
            // Quand le noeud reçoit le message : alors il va le transférer.

            /*
            var node = PartitionedNodeHostFactory.New(options: p =>
            {
                p.Port = 5000;
            })
                .UseUDPTransport()
                // .UseDirectPartitionKey(new DirectPartitionPeer { Port = 3000, PartitionKey = "Key" }, new DirectPartitionPeer {  Port = 3001, PartitionKey = "Key2" })
                .Build();
            node.Start().Wait();

            // Il faut modifier la classe IClusterStore pour récupérer le noeud.
            // Ajouter une nouvelle partition 
            // Le noeud va posséder un ou plusieurs PEER.
            // Chaque Peer possède son propre moyen de découverte et va communiquer avec les autres noeuds.

            /*
            var firstPeer = LaunchRaftConsensusPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5002) }, "node1", 5001);
            var secondPeer = LaunchRaftConsensusPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5001) }, "node2", 5002);
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
            */
            Console.WriteLine("Press any key to quit the application");
            Console.ReadLine();
            // firstPeer.Stop();
            // secondPeer.Stop();
            return 1;
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
                var peerState = await raftConsensusClient.GetPeerState(CancellationToken.None);
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
                var result = await raftConsensusClient.GetLogs(1, CancellationToken.None);
                foreach(var log in result.Entries)
                {
                    Console.WriteLine($"Index {log.Index}");
                    Console.WriteLine($"Term {log.Term}");
                }
            }
        }
    }
}
