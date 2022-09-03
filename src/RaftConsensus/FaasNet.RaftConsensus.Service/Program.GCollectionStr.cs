using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.StateMachines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;

namespace FaasNet.RaftConsensus.Service
{
    internal partial class Program
    {
        /*
        public static void LaunchGCollectionStr()
        {
            LaunchRaftConsensusPeerGCollectionStr(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5002) }, false, "node1", 5001);
            LaunchRaftConsensusPeerGCollectionStr(new ConcurrentBag<ClusterPeer> { new ClusterPeer("localhost", 5001) }, false, "node2", 5002);

            Console.WriteLine("Press any key to add a string into the collection");
            Console.ReadLine();
            AddStr(5001, "Hello", false);

            Console.WriteLine("Press any key to add a second string into the collection");
            Console.ReadLine();
            AddStr(5001, "Hello 2", false);

            Console.WriteLine("Press any key to display the state machine");
            Console.ReadLine();
            DisplayGCollectionStateMachine(5001, false);

            Console.WriteLine("Press any key to remove the first string from the collection");
            Console.ReadLine();
            RemoveStr(5001, "Hello", false);

            Console.WriteLine("Press any key to display the state machine");
            Console.ReadLine();
            DisplayGCollectionStateMachine(5001, false);

            Console.WriteLine("Press any key to display state of Peer 5001");
            Console.ReadLine();
            DisplayPeerState(5001, false);

            Console.WriteLine("Press any key to display state of Peer 5002");
            Console.ReadLine();
            DisplayPeerState(5002, false);
        }

        private static IPeerHost LaunchRaftConsensusPeerGCollectionStr(ConcurrentBag<ClusterPeer> clusterPeers, bool isTcp = false, string nodeName = "node1", int port = 5001)
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
                    o.StateMachineType = typeof(GCollection);
                    o.SnapshotFrequency = 2;
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

        private static async void AddStr(int port, string value, bool isTcp = false)
        {
            using (var client = PeerClientFactory.Build<RaftConsensusClient>("localhost", port, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
                await client.SendCommand(new AddStringEntityCommand { Record = new StringStateMachine { Id = value } }, 1000000);

        }

        private static async void RemoveStr(int port, string value, bool isTcp = false)
        {
            using (var client = PeerClientFactory.Build<RaftConsensusClient>("localhost", port, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
                await client.SendCommand(new RemoveEntityCommand { Id = value }, 1000000);
        }

        private static async void DisplayGCollectionStateMachine(int port, bool isTcp = false)
        {
            using (var client = PeerClientFactory.Build<RaftConsensusClient>("localhost", port, isTcp ? ClientTransportFactory.NewTCP() : ClientTransportFactory.NewUDP()))
            {
                Console.WriteLine("GCollection");
                var stateMachine = (await client.GetStateMachine(1000000)).First();
                var result = StateMachineSerializer.Deserialize<GCollection>(stateMachine.StateMachine);
                foreach(var record in result.Values) Console.WriteLine(record.ToString());
                Console.WriteLine();
            }
        }
        */
    }
}
