using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

var allNodes = new List<INodeHost>();
var firstNode = BuildNodeHost(new ConcurrentBag<PeerInfo>
{
    new PeerInfo { TermId = "termId", ConfirmedTermIndex = 0 },
    new PeerInfo { TermId = "secondTermId", ConfirmedTermIndex = 0 }
}, 4001, new ConcurrentBag<ClusterNode>
{
    new ClusterNode
    {
        Port = 4002,
        Url = "localhost"
    }
});
var secondNode = BuildNodeHost(new ConcurrentBag<PeerInfo>
{
    new PeerInfo { TermId = "termId", ConfirmedTermIndex = 0 },
    new PeerInfo { TermId = "secondTermId", ConfirmedTermIndex = 0 }
}, 4002, new ConcurrentBag<ClusterNode>
{
    new ClusterNode
    {
        Port = 4001,
        Url = "localhost"
    }
});
allNodes.Add(firstNode);
allNodes.Add(secondNode);

await firstNode.Start(CancellationToken.None);
await secondNode.Start(CancellationToken.None);

// Wait leaders have been chosen.
WaitOnlyOneLeader(allNodes, "termId");
WaitOnlyOneLeader(allNodes, "secondTermId");

// Append logs.
var client = new ConsensusClient("localhost", 4001);
client.AppendEntry("termId", "Key", "value", CancellationToken.None).Wait();
client = new ConsensusClient("localhost", 4001);
client.AppendEntry("secondTermId", "SecondKey", "value", CancellationToken.None).Wait();
WaitLogs(allNodes, p => p.Info.TermId == "termId", l => l.Key == "Key");
WaitLogs(allNodes, p => p.Info.TermId == "secondTermId", l => l.Key == "SecondKey");

Console.WriteLine("Press Enter to quit the application");
Console.ReadLine();

static INodeHost BuildNodeHost(ConcurrentBag<PeerInfo> peers, int port, ConcurrentBag<ClusterNode> clusterNodes)
{
    var serviceProvider = new ServiceCollection()
        .AddConsensusPeer(o => o.Port = port)
        .SetPeers(peers)
        .SetClusterNodes(clusterNodes)
        .Services
        .AddLogging(l => l.AddConsole())
        .BuildServiceProvider();
    return serviceProvider.GetService<INodeHost>();
}

static void WaitOnlyOneLeader(List<INodeHost> nodes, string termId)
{
    while (true)
    {
        var peers = nodes.SelectMany(n => n.Peers).Where(p => p.Info.TermId == termId);
        var leaderNodes = peers.Where(p => p.State == PeerStates.LEADER);
        var followerNodes = peers.Where(p => p.State == PeerStates.FOLLOWER && p.ActiveNode != null);
        if (leaderNodes.Count() == 1 && followerNodes.Count() == peers.Count() - 1)
        {
            return;
        }

        Thread.Sleep(200);
    }
}

static void WaitLogs(List<INodeHost> nodes, Func<IPeerHost, bool> callbackPeers, Func<LogRecord, bool> callbackLogRecords)
{
    var isLogPropagated = false;
    while (!isLogPropagated)
    {
        isLogPropagated = true;
        foreach (var node in nodes)
        {
            var filteredPeers = node.Peers.Where(callbackPeers);
            foreach (var filteredPeer in filteredPeers)
            {
                var filteredLogs = filteredPeer.LogStore.GetAll(CancellationToken.None).Result.Where(callbackLogRecords);
                if (!filteredLogs.Any()) isLogPropagated = false;
            }
        }

        Thread.Sleep(200);
    }
}