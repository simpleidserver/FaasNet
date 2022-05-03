using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

var allNodes = new List<INodeHost>();
var firstNode = BuildNodeHost(new ConcurrentBag<PeerInfo>
{
    new PeerInfo { TermId = "termId", ConfirmedTermIndex = 0 }
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
    new PeerInfo { TermId = "termId", ConfirmedTermIndex = 0 }
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

// Four different nodes.
// Each node contains one partition.
await firstNode.Start(CancellationToken.None);
await secondNode.Start(CancellationToken.None);

// Select the leader.
IPeerHost? leaderHost = null;
while (leaderHost == null)
{
    var leaderNodes = allNodes.SelectMany(n => n.Peers).Where(p => p.State == PeerStates.LEADER);
    if (leaderNodes.Any() || leaderNodes.Count() == 1)
    {
        leaderHost = leaderNodes.First();
    }

    Thread.Sleep(200);
}

Thread.Sleep(2000);

// Append entry.
var client = new ConsensusClient("localhost", 4001);
client.AppendEntry("termId", "Key", "value", CancellationToken.None).Wait();

var isLogPropagated = false;
while(!isLogPropagated)
{
    isLogPropagated = true;
    foreach (var nodes in allNodes)
    {
        var logs = nodes.Peers.First().LogStore.GetAll(CancellationToken.None).Result;
        if (!logs.Any() || logs.Count() != 1) isLogPropagated = false;
    }

    Thread.Sleep(200);
}

string ss = "";

// Wait at least one leader is selected.
#pragma warning disable 4014
#pragma warning restore 4014

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