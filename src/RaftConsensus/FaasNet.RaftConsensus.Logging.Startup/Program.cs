
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

int seedPort = 4000, nbNodeStarted = 0;

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
for (int i = 0; i <= nbNodes; i++)
{
    var clusterNodes = new ConcurrentBag<NodeState>();
    allNodes.Add(BuildNodeHost(
        new ConcurrentBag<PeerInfo>(peerInfos.Select(p => new PeerInfo { TermId = p })),
        seedPort + i,
        Path.Combine(Directory.GetCurrentDirectory(), i + "-log-{0}.txt")));
}

for(int i =0; i <= nbNodes; i++)
{
    var node = allNodes[i];
    await node.Start(CancellationToken.None);
    foreach (var peer in node.Peers)
    {
        peer.PeerIsFollower += (s, e) => Console.WriteLine($"Node '{e.NodeId}', Peer {e.PeerId}, Term {e.TermId} is follower");
        peer.PeerIsCandidate += (s, e) => Console.WriteLine($"Node '{e.NodeId}', Peer {e.PeerId}, Term {e.TermId} is candidate");
        peer.PeerIsLeader += (s, e) => Console.WriteLine($"Node '{e.NodeId}', Peer {e.PeerId}, Term {e.TermId} is leader");
    }

    node.NodeStarted += (s, e) =>
    {
        nbNodeStarted++;
        if (nbNodeStarted == nbNodes + 1)
        {
            for(int y = 1; y <= nbNodes; y++)
            {
                using (var gossipClient = new GossipClient("localhost", seedPort))
                {
                    gossipClient.JoinNode("localhost", seedPort + 1);
                }
            }
        }
    };
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
    var consensusClient = new ConsensusClient("localhost", seedPort);
    await consensusClient.AppendEntry(termId, message, CancellationToken.None);
}
while (continueExecution);

Console.WriteLine("Press Enter to quit the application");
Console.ReadLine();

foreach (var node in allNodes) await node.Stop();

static INodeHost BuildNodeHost(ConcurrentBag<PeerInfo> peers, int port, string logPath)
{
    var serviceProvider = new ServiceCollection()
        .AddConsensusPeer(o => o.Port = port)
        // .SetPeers(peers)
        // .SetNodeStates(nodeStates)
        .UseLogFileStore(o => o.LogFilePath = logPath)
        .Services
        // .AddLogging(l => l.AddConsole())
        .BuildServiceProvider();
    return serviceProvider.GetService<INodeHost>();
}