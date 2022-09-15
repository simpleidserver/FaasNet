using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.StateMachines.Counter;
using FaasNet.RaftConsensus.Core.StateMachines.Counter;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;

namespace FaasNet.RaftConsensus.Service
{
    internal partial class Program
    {
        public static void LaunchCounter()
        {
            LaunchRaftConsensusPeerCounter(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5002) }, false, "node1", 5001);
            LaunchRaftConsensusPeerCounter(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5001) }, false, "node2", 5002);

            Console.WriteLine("Press any key to increment the counter 'firstCounter' by 2");
            Console.ReadLine();
            IncrementCounter("firstCounter", 5001, 2, false);

            Console.WriteLine("Press any key to increment the counter 'firstCounter' by 3");
            Console.ReadLine();
            IncrementCounter("firstCounter", 5001, 3, false);

            Console.WriteLine("Press any key to increment the counter 'firstCounter' by 5");
            Console.ReadLine();
            IncrementCounter("firstCounter", 5001, 5, false);

            Console.WriteLine("Press any key to increment the counter 'secondCounter' by 1");
            Console.ReadLine();
            IncrementCounter("secondCounter", 5001, 1, false);

            Console.WriteLine("Press any key to display the counter 'firstCounter'");
            Console.ReadLine();
            DisplayCounter("firstCounter", 5001, false);

            Console.WriteLine("Press any key to display the counter 'secondCounter'");
            Console.ReadLine();
            DisplayCounter("secondCounter", 5001, false);
        }

        private static IPeerHost LaunchRaftConsensusPeerCounter(ConcurrentBag<ClusterPeer> clusterPeers, bool isTcp = false, string nodeName = "node1", int port = 5001)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var peerHostFactory = PeerHostFactory.NewUnstructured(o =>
            {
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
                    o.StateMachineType = typeof(CounterStateMachine);
                    o.IsConfigurationStoredInMemory = false;
                    o.SnapshotFrequency = 1;
                    o.ConfigurationDirectoryPath = Path.Combine(path, nodeName);
                    o.LeaderCallback = () =>
                    {
                        Console.WriteLine("There a is a leader");
                    };
                });
            if (isTcp) peerHostFactory.UseTCPTransport();
            else peerHostFactory.UseUDPTransport();
            var peerHost = peerHostFactory.Build();
            peerHost.Start();
            return peerHost;
        }

        private static async void IncrementCounter(string id, int port, long value, bool isTcp = false)
        {
            using (var client = PeerClientFactory.Build<RaftConsensusClient>("localhost", port, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
                await client.SendCommand(new IncrementCounter { Id = id, Value = value }, 1000000);
        }

        private static async void DisplayCounter(string id, int port, bool isTcp = false)
        {
            using (var client = PeerClientFactory.Build<RaftConsensusClient>("localhost", port, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                var result = (await client.ExecuteQuery(new GetCounterQuery { Id = id }, 1000000)).First();
                var counterQueryResult = result.Item1.Result as GetCounterQueryResult;
                Console.WriteLine($"{id} = {counterQueryResult.Value}");
            }
        }

        private static async void DisplayPeerState(int port, bool isTcp = false)
        {
            using (var client = PeerClientFactory.Build<RaftConsensusClient>("localhost", port, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                var peerState = (await client.GetPeerState(5000)).First().Item1;
                Console.WriteLine($"Status {peerState.Status}");
                Console.WriteLine($"Term {peerState.Term}");
                Console.WriteLine($"CommitIndex {peerState.CommitIndex}");
                Console.WriteLine($"LastApplied {peerState.LastApplied}");
                Console.WriteLine($"VotedFor {peerState.VotedFor}");
                Console.WriteLine($"SnapshotLastApplied {peerState.SnapshotLastApplied}");
                Console.WriteLine($"SnapshotCommitIndex {peerState.SnapshotCommitIndex}");
            }
        }
    }
}
