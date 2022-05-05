
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

int initPort = 4000;

Console.WriteLine("How many nodes do you want to start ?");
int nbNodes = int.Parse(Console.ReadLine());

var peerInfos = new ConcurrentBag<string>();
var continueExecution = true;
do
{
    Console.WriteLine("Enter 'Q' to stop adding new peer or enter the name of your peer");
    string str = Console.ReadLine();
    continueExecution = str != "Q";
    if (continueExecution) peerInfos.Add(str);
}
while (continueExecution);

var allNodes = new List<INodeHost>();
for (int i = 0; i < nbNodes; i++)
{
    var clusterNodes = new ConcurrentBag<ClusterNode>();
    for (int y = 0; y < nbNodes; y++) if (y != i) clusterNodes.Add(new ClusterNode { Url = "localhost", Port = initPort + y });
    allNodes.Add(BuildNodeHost(
        new ConcurrentBag<PeerInfo>(peerInfos.Select(p => new PeerInfo { TermId = p })),
        initPort + i,
        clusterNodes,
        Path.Combine(Directory.GetCurrentDirectory(), i + "-log-{0}.txt")));
}

foreach (var node in allNodes)
{
    await node.Start(CancellationToken.None);
    foreach(var peer in node.Peers)
    {
        peer.PeerIsFollower += (s, e) => Console.WriteLine($"Node '{e.NodeId}', Peer {e.PeerId}, Term {e.TermId} is follower");
        peer.PeerIsCandidate += (s, e) => Console.WriteLine($"Node '{e.NodeId}', Peer {e.PeerId}, Term {e.TermId} is candidate");
        peer.PeerIsLeader += (s, e) => Console.WriteLine($"Node '{e.NodeId}', Peer {e.PeerId}, Term {e.TermId} is leader");
    }
}

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
    var consensusClient = new ConsensusClient("localhost", initPort);
    await consensusClient.AppendEntry(termId, message, CancellationToken.None);
}
while (continueExecution);

Console.WriteLine("Press Enter to quit the application");
Console.ReadLine();

foreach (var node in allNodes) await node.Stop();

static INodeHost BuildNodeHost(ConcurrentBag<PeerInfo> peers, int port, ConcurrentBag<ClusterNode> clusterNodes, string logPath)
{
    var serviceProvider = new ServiceCollection()
        .AddConsensusPeer(o => o.Port = port)
        .SetPeers(peers)
        .SetClusterNodes(clusterNodes)
        .UseLogFileStore(o => o.LogFilePath = logPath)
        .Services
        // .AddLogging(l => l.AddConsole())
        .BuildServiceProvider();
    return serviceProvider.GetService<INodeHost>();
}