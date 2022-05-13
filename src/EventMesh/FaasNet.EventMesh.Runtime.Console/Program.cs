using FaasNet.Common;
using FaasNet.EventMesh.Client;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages.Consensus;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

int seedPort = 4000;
var allNodes = CreateNodes();
await StartNodes(allNodes);
await DisplayMenu(allNodes);
Console.WriteLine("Press Enter to quit the application");
Console.ReadLine();

foreach (var node in allNodes) await node.Stop();

ICollection<INodeHost> CreateNodes()
{
    Console.WriteLine("How many nodes do you want to start ?");
    int nbNodes = int.Parse(Console.ReadLine());
    var allNodes = new List<INodeHost>();
    for (int i = 0; i <= nbNodes; i++)
    {
        allNodes.Add(BuildNodeHost(
            seedPort + i,
            i == 0));
    }

    return allNodes;
}

async Task StartNodes(ICollection<INodeHost> nodes)
{
    int nbNodes = nodes.Count();
    for (int i = 0; i < nbNodes; i++)
    {
        await StartNode(nodes.ElementAt(i), nbNodes);
    }
}

async Task StartNode(INodeHost node, int nbNodes)
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
        if (node.Port == seedPort) return;
        using (var gossipClient = new GossipClient("localhost", seedPort))
        {
            gossipClient.JoinNode("localhost", node.Port);
        }
    };
}

async Task DisplayMenu(ICollection<INodeHost> nodes)
{
    var continueExecution = true;
    do
    {
        Console.WriteLine("- Enter 'Q' to stop execution");
        Console.WriteLine("- Enter 'states' to display the states");
        Console.WriteLine("- Enter 'peers' to display the peers");
        Console.WriteLine("- Enter 'addvpn' to add a VPN");
        Console.WriteLine("- Enter 'addclient' to add a client");
        Console.WriteLine("- Enter 'publishmsg' to publish a message");
        Console.WriteLine("- Enter 'submsg' to subscribe");
        // Pouvoir souscrire à un topic.
        // Pouvoir publier sur un topic.
        // Le seed kafka peut directement publier sur eventmesh.
        string menuId = Console.ReadLine();
        continueExecution = menuId != "Q";
        if (menuId == "states")
        {
            foreach (var node in nodes)
            {
                var entityTypes = await node.NodeStateStore.GetAllEntityTypes(CancellationToken.None);
                foreach (var entityType in entityTypes)
                {
                    Console.WriteLine($"Port = {node.Port}, Type = {entityType.EntityType}, Version = {entityType.EntityVersion}, Value = {entityType.Value}");
                }
            }

            continue;
        }

        if (menuId == "peers")
        {
            foreach (var node in allNodes)
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
            var eventMeshClient = new EventMeshClient("localhost", seedPort);
            await eventMeshClient.AddVpn(vpn, CancellationToken.None);
            continue;
        }

        if (menuId == "addclient")
        {
            Console.WriteLine("Enter the VPN");
            string vpn = Console.ReadLine();
            Console.WriteLine("Enter the client identifier");
            string clientId = Console.ReadLine();
            var eventMeshClient = new EventMeshClient("localhost", seedPort);
            await eventMeshClient.AddClient(vpn, clientId, new List<UserAgentPurpose> { UserAgentPurpose.SUB, UserAgentPurpose.PUB }, CancellationToken.None);
            continue;
        }

        if (menuId == "publishmsg")
        {
            Console.WriteLine("Enter the VPN");
            var vpn = Console.ReadLine();
            Console.WriteLine("Enter the client identifier");
            var clientIdentifier = Console.ReadLine();
            var eventMeshClient = new EventMeshClient("localhost", seedPort);
            var session = await eventMeshClient.CreatePubSession(vpn, clientIdentifier, CancellationToken.None);
            await session.Publish("person.created", new { firstName = "firstName" }, CancellationToken.None);
            continue;
        }

        if (menuId == "submsg")
        {
            Console.WriteLine("Enter the VPN");
            var vpn = Console.ReadLine();
            Console.WriteLine("Enter the client identifier");
            var clientIdentifier = Console.ReadLine();
            var eventMeshClient = new EventMeshClient("localhost", seedPort);
            var session = await eventMeshClient.CreateSubSession(vpn, clientIdentifier, CancellationToken.None);
            await session.Subscribe("person.created", (ce) =>
            {
                Console.WriteLine("COUCOU");
            }, CancellationToken.None);
            continue;
        }
    }
    while (continueExecution);
}

INodeHost BuildNodeHost(int port, bool isSeed = false)
{
    var serverBuilder = new ServiceCollection()
        .AddEventMeshServer(o => o.Port = port);
    if (isSeed) serverBuilder.SetNodeStates(new ConcurrentBag<NodeState> { new ClusterNode { Port = 4000, Url = "localhost" }.ToNodeState() });
    var serviceProvider = serverBuilder.Services
        // .AddLogging(l => l.AddConsole())
        .BuildServiceProvider();
    return serviceProvider.GetRequiredService<INodeHost>();
}