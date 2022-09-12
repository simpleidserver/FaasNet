using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.StateMachines;
using FaasNet.RaftConsensus.Core.StateMachines;
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
            LaunchRaftConsensusPeerGCounter(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5002) }, false, "node1", 5001);
            LaunchRaftConsensusPeerGCounter(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5001) }, false, "node2", 5002);

            Console.WriteLine("Press any key to increment the counter 'firstCounter' by 2");
            Console.ReadLine();
            IncrementCounter("firstCounter", 5001, 2, false);

            Console.WriteLine("Press any key to display the state machine 'firstCounter'");
            Console.ReadLine();
            DisplayGCounterStateMachine("firstCounter", 5001, false);

            Console.WriteLine("Press any key to increment the counter 'secondCounter' by 3");
            Console.ReadLine();
            IncrementCounter("secondCounter", 5001, 3, false);

            Console.WriteLine("Press any key to display the state machine 'secondCounter'");
            Console.ReadLine();
            DisplayGCounterStateMachine("secondCounter", 5001, false);

            Console.WriteLine("Press any key to display state of Peer 5001");
            Console.ReadLine();
            DisplayPeerState(5001, false);

            Console.WriteLine("Press any key to display state of Peer 5002");
            Console.ReadLine();
            DisplayPeerState(5002, false);
        }

        private static IPeerHost LaunchRaftConsensusPeerGCounter(ConcurrentBag<ClusterPeer> clusterPeers, bool isTcp = false, string nodeName = "node1", int port = 5001)
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
                    o.IsConfigurationStoredInMemory = true;
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
                await client.SendCommand(id, new IncrementGCounterCommand { Value = value }, 1000000);
        }

        private static async void DisplayGCounterStateMachine(string id, int port, bool isTcp = false)
        {
            using (var client = PeerClientFactory.Build<RaftConsensusClient>("localhost", port, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                var stateMachine = (await client.GetStateMachine(id, 1000000)).First();
                var result = StateMachineSerializer.Deserialize<CounterStateMachine>(stateMachine.Item1.StateMachine);
                Console.WriteLine($"Value  = {result.Value}");
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
