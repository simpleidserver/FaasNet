
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

int seedPort = 4000;
var allNodes = CreateNodes();
await StartNodes(allNodes);
await DisplayMenu(allNodes);

/*
var peerInfos = new ConcurrentBag<string>();
do
{
    Console.WriteLine("Enter 'Q' to stop adding new peer or enter the name of your peer");
    string str = Console.ReadLine();
    continueExecution = str != "Q";
    if (continueExecution) peerInfos.Add(str);
}
while (continueExecution);

continueExecution = true;
do
{
    Console.WriteLine("Enter 'Q' to stop sending message / Enter 'inf' to display the status of the peers / Enter the name of the partition");
    string termId = Console.ReadLine();
    continueExecution = termId != "Q";
    if(termId == "inf")
    {
        foreach(var node in allNodes)
        {
            foreach(var peer in node.Peers)
            {
                Console.WriteLine($"Node '{node.NodeId}', Peer '{peer.PeerId}', TermId '{peer.Info.TermId}', TermIndex '{peer.Info.ConfirmedTermIndex}', State '{peer.State}");
            }
        }

        continue;
    }

    Console.WriteLine("Please enter the message");
    string message = Console.ReadLine();
    var consensusClient = new ConsensusClient("localhost", seedPort);
    await consensusClient.AppendEntry(termId, message, CancellationToken.None);
}
while (continueExecution);
*/

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
            new ConcurrentBag<PeerInfo>(/*peerInfos.Select(p => new PeerInfo { TermId = p })*/),
            seedPort + i,
            Path.Combine(Directory.GetCurrentDirectory(), i + "-log-{0}.txt"),
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
        Console.WriteLine("Enter 'Q' to stop execution / Enter 'inf' to display the states / Start a new node 'addnode'");
        string termId = Console.ReadLine();
        continueExecution = termId != "Q";
        if (termId == "inf")
        {
            foreach (var node in nodes)
            {
                var entityTypes = await node.NodeStateStore.GetAllEntityTypes(CancellationToken.None);
                foreach(var entityType in entityTypes)
                {
                    Console.WriteLine($"Port = {node.Port}, Type = {entityType.EntityType}, Version = {entityType.EntityVersion}, Value = {entityType.Value}");
                }
            }

            continue;
        }

        if(termId == "addnode")
        {
            var newNode = BuildNodeHost(
                new ConcurrentBag<PeerInfo>(/*peerInfos.Select(p => new PeerInfo { TermId = p })*/),
                seedPort + nodes.Count(),
                Path.Combine(Directory.GetCurrentDirectory(), nodes.Count() + "-log-{0}.txt"),
                false);
            await StartNode(newNode, nodes.Count());
            nodes.Add(newNode);
        }
    }
    while (continueExecution);
}

INodeHost BuildNodeHost(ConcurrentBag<PeerInfo> peers, int port, string logPath, bool isSeed = false)
{
    var services = new ServiceCollection()
        .AddConsensusPeer(o => o.Port = port);
    if (isSeed) services.SetNodeStates(new ConcurrentBag<NodeState> { new ClusterNode { Port = 4000, Url = "localhost" }.ToNodeState() });
        // .SetPeers(peers)
        // .SetNodeStates(nodeStates)
    var serviceProvider = services.UseLogFileStore(o => o.LogFilePath = logPath)
        .Services
        // .AddLogging(l => l.AddConsole())
        .BuildServiceProvider();
    return serviceProvider.GetRequiredService<INodeHost>();
}