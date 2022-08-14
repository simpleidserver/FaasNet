using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace FaasNet.RaftConsensus.Service
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var firstPeer = LaunchRaftConsensusPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5002) }, "node1", 5001);
            var secondPeer = LaunchRaftConsensusPeer(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5001) }, "node2", 5002);
            Console.WriteLine("Press any key to add an entry");
            Console.ReadLine();
            AddLogEntry(5002);
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

        private static async void AddLogEntry(int port)
        {
            using (var raftConsensusClient = new UDPRaftConsensusClient("localhost", port))
            {
                await raftConsensusClient.AppendEntry(Encoding.UTF8.GetBytes("value"), CancellationToken.None);
            }
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
