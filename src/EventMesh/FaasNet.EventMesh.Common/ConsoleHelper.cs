using FaasNet.Common;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Protocols;
using FaasNet.EventMesh.Seed;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Common
{
    public class ConsoleHelper
    {
        private static int _seedPort;
        private static int _amqpPort = 5672;
        private static int _nbNode;
        private static List<INodeHost> _allNodes;

        public static async Task Start(int seedPort, int amqpPort = 5672)
        {
            // Ajouter des tests unitaires.
            // Supporter WS-Socket.
            _seedPort = seedPort;
            _amqpPort = amqpPort;
            _nbNode = 1;
            _allNodes = new List<INodeHost> { BuildNodeHost(seedPort, true) };
            await StartAMQPProtocol();
            await StartNodes(_allNodes);
            await DisplayMenu(_allNodes);
        }

        private static async Task StartAMQPProtocol()
        {
            var serviceCollection = new ServiceCollection();
            var serverBuilder = new ServerBuilder(serviceCollection);
            serverBuilder.UseAMQPProtocol(o =>
            {
                o.Port = _amqpPort;
                o.EventMeshPort = _seedPort;
            });
            var serviceProvider = serverBuilder.Services.BuildServiceProvider();
            var proxy = serviceProvider.GetRequiredService<IProxy>();
            await proxy.Start();
        }

        private static async Task StartNodes(IEnumerable<INodeHost> allNodes)
        {
            for (int i = 0; i < allNodes.Count(); i++) await StartNode(allNodes.ElementAt(i), i);
        }

        private static async Task StartNode(INodeHost node, int nbNodes)
        {
            await node.Start(CancellationToken.None);
            foreach (var peer in node.Peers)
            {
                peer.PeerIsFollower += (s, e) => Console.WriteLine($"Node '{e.NodeId}', Peer {e.PeerId}, Term {e.TermId} is follower");
                peer.PeerIsCandidate += (s, e) => Console.WriteLine($"Node '{e.NodeId}', Peer {e.PeerId}, Term {e.TermId} is candidate");
                peer.PeerIsLeader += (s, e) => Console.WriteLine($"Node '{e.NodeId}', Peer {e.PeerId}, Term {e.TermId} is leader");
            }

            node.NodeStarted += (s, e) =>
            {
                if (node.Port == _seedPort) return;
                using (var gossipClient = new GossipClient("localhost", _seedPort))
                {
                    gossipClient.JoinNode("localhost", node.Port);
                }
            };
        }

        private static async Task StopNodes(IEnumerable<INodeHost> allNodes)
        {
            foreach (var node in allNodes) await node.Stop();
        }

        private static async Task DisplayMenu(ICollection<INodeHost> nodes)
        {
            var continueExecution = true;
            do
            {
                Console.WriteLine("- Enter 'Q' to stop execution");
                Console.WriteLine("- Enter 'addnode' to add a node");
                Console.WriteLine("- Enter 'states' to display the states");
                Console.WriteLine("- Enter 'peers' to display the peers");
                Console.WriteLine("- Enter 'addvpn' to add a VPN");
                Console.WriteLine("- Enter 'addclient' to add a client");
                Console.WriteLine("- Enter 'addvpnbridge' to add VPN bridge");
                Console.WriteLine("- Enter 'publishmsg' to publish a message");
                Console.WriteLine("- Enter 'persistedsub' to subscribe to a group identifier");
                Console.WriteLine("- Enter 'directsub' to subscribe to a topic");
                Console.WriteLine("- Enter 'startamqpseed' to listen amqp");
                Console.WriteLine("- Enter 'startkafkaseed' to listen kafka");
                Console.WriteLine("- Enter 'startvpnbridgeseed' to listen VPN bridge");
                string menuId = Console.ReadLine();
                continueExecution = menuId != "Q";
                if (menuId == "addnode")
                {
                    int currentPort = _seedPort + _nbNode;
                    var newHost = BuildNodeHost(currentPort, false);
                    await StartNode(newHost, _nbNode);
                    _allNodes.Add(newHost);
                    _nbNode++;
                    continue;
                }

                if (menuId == "states")
                {
                    foreach (var node in nodes)
                    {
                        var entityTypes = await node.NodeStateStore.GetAllLastEntityTypes(CancellationToken.None);
                        foreach (var entityType in entityTypes)
                        {
                            Console.WriteLine($"Port = {node.Port}, Type = {entityType.EntityType}, Version = {entityType.EntityVersion}, Value = {entityType.Value}");
                        }
                    }

                    continue;
                }

                if (menuId == "peers")
                {
                    foreach (var node in _allNodes)
                    {
                        foreach (var peer in node.Peers)
                        {
                            Console.WriteLine($"Node '{node.NodeId}', Peer '{peer.PeerId}', TermId '{peer.Info.TermId}', TermIndex '{peer.Info.ConfirmedTermIndex}', State '{peer.State}");
                        }
                    }
                    continue;
                }

                if (menuId == "addvpn")
                {
                    Console.WriteLine("Enter the VPN");
                    string vpn = Console.ReadLine();
                    var eventMeshClient = new EventMeshClient("localhost", _seedPort);
                    await eventMeshClient.AddVpn(vpn, CancellationToken.None);
                    continue;
                }

                if (menuId == "addclient")
                {
                    Console.WriteLine("Enter the VPN");
                    string vpn = Console.ReadLine();
                    Console.WriteLine("Enter the client identifier");
                    string clientId = Console.ReadLine();
                    var eventMeshClient = new EventMeshClient("localhost", _seedPort);
                    await eventMeshClient.AddClient(vpn, clientId, new List<UserAgentPurpose> { UserAgentPurpose.SUB, UserAgentPurpose.PUB }, CancellationToken.None);
                    continue;
                }

                if (menuId == "addvpnbridge")
                {
                    Console.WriteLine("Enter source VPN");
                    var sourceVpn = Console.ReadLine();
                    Console.WriteLine("Enter the target port");
                    var targetPort = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter the target VPN");
                    var targetVpn = Console.ReadLine();
                    var eventMeshClient = new EventMeshClient("localhost", _seedPort);
                    await eventMeshClient.AddBridge(sourceVpn, "localhost", targetPort, targetVpn);
                }

                if (menuId == "publishmsg")
                {
                    Console.WriteLine("Enter the VPN");
                    var vpn = Console.ReadLine();
                    Console.WriteLine("Enter the client identifier");
                    var clientIdentifier = Console.ReadLine();
                    var eventMeshClient = new EventMeshClient("localhost", _seedPort);
                    var session = await eventMeshClient.CreatePubSession(vpn, clientIdentifier, null, CancellationToken.None);
                    await session.Publish("person.created", new { firstName = "firstName" }, CancellationToken.None);
                    continue;
                }

                if (menuId == "persistedsub")
                {
                    Console.WriteLine("Enter the VPN");
                    var vpn = Console.ReadLine();
                    Console.WriteLine("Enter the client identifier");
                    var clientIdentifier = Console.ReadLine();
                    Console.WriteLine("Enter the group identifier");
                    var groupId = Console.ReadLine();
                    var eventMeshClient = new EventMeshClient("localhost", _seedPort);
                    var session = await eventMeshClient.CreateSubSession(vpn, clientIdentifier, null, CancellationToken.None);
                    await session.PersistedSubscribe("person.created", groupId, (ce) =>
                    {
                        Console.WriteLine("Persisted sub");
                    }, CancellationToken.None);
                    continue;
                }

                if (menuId == "directsub")
                {
                    Console.WriteLine("Enter the VPN");
                    var vpn = Console.ReadLine();
                    Console.WriteLine("Enter the client identifier");
                    var clientIdentifier = Console.ReadLine();
                    var eventMeshClient = new EventMeshClient("localhost", _seedPort);
                    var session = await eventMeshClient.CreateSubSession(vpn, clientIdentifier, null, CancellationToken.None);
                    session.DirectSubscribe("person.created", (ce) =>
                    {
                        Console.WriteLine($"Direct sub {ce.Data}");
                    }, CancellationToken.None);
                    continue;
                }

                if (menuId == "startamqpseed")
                {
                    await StartAMQPSeed();
                }

                if (menuId == "startkafkaseed")
                {
                    await StartKafkaSeed();
                }

                if (menuId == "startvpnbridgeseed")
                {
                    await StartVpnBridgeSeed();
                }
            }
            while (continueExecution);
        }

        private static INodeHost BuildNodeHost(int port, bool isSeed = false)
        {
            var serverBuilder = new ServiceCollection()
                .AddEventMeshServer(consensusNodeCallback: o => o.Port = port)
                .UseRocksDB(o => { o.SubPath = $"node{port}"; });
            // if (isSeed) serverBuilder.SetNodeStates(new ConcurrentBag<NodeState> { new ClusterNode { Port = 4000, Url = "localhost" }.ToNodeState() });
            var serviceProvider = serverBuilder.Services
                // .AddLogging(l => l.AddConsole())
                .BuildServiceProvider();
            // if(isSeed)
            {
                var nodeStateStore = serviceProvider.GetRequiredService<INodeStateStore>();
                var clusterStore = serviceProvider.GetRequiredService<IClusterStore>();
                if (!clusterStore.GetAllNodes(CancellationToken.None).Result.Any())
                {
                    clusterStore.AddNode(new ClusterNode { Port = _seedPort, Url = "localhost" }, CancellationToken.None).Wait();
                }
            }

            return serviceProvider.GetRequiredService<INodeHost>();
        }

        private static async Task StartAMQPSeed()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddAMQPSeed(seed =>
            {
                seed.EventMeshPort = _seedPort;
                seed.Vpn = "default";
                seed.ClientId = "clientId";
                seed.EventMeshUrl = "localhost";
            }, amqp =>
            {
                amqp.ConnectionFactory = (c) =>
                {
                    c.UserName = "default_user_TBRVU8sYJmyVBXKeiTD";
                    c.Password = "d67v-U305u8Sh0dOwn02pTIuo2jsnLwY";
                    c.Port = 30007;
                };
            }).UseSeedRocksDB();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var seedJob = serviceProvider.GetRequiredService<ISeedJob>();
            await seedJob.Start(CancellationToken.None);
        }

        private static async Task StartVpnBridgeSeed()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddVpnBridgeSeed(o =>
            {
                o.EventMeshPort = _seedPort;
            }).UseSeedRocksDB(); ;
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var seedJob = serviceProvider.GetRequiredService<ISeedJob>();
            await seedJob.Start(CancellationToken.None);
        }

        private static async Task StartKafkaSeed()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddKafkaSeed(seed =>
            {
                seed.EventMeshPort = _seedPort;
                seed.Vpn = "default";
                seed.ClientId = "clientId";
                seed.EventMeshUrl = "localhost";
            }, kafka =>
            {
                kafka.BootstrapServers = "localhost:29092";
            }).UseSeedRocksDB();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var seedJob = serviceProvider.GetRequiredService<ISeedJob>();
            await seedJob.Start(CancellationToken.None);
        }
    }
}