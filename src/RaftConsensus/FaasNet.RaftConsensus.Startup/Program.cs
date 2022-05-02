using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

var firstNode = BuildNodeHost(new ConcurrentBag<PeerInfo>
{
    new PeerInfo { TermId = "termId", TermIndex = 0 }
}, 4001, new ConcurrentBag<ClusterNode>
{
    new ClusterNode
    {
        Port = 4002,
        Url = "localhost"
    },
    new ClusterNode
    {
        Port = 4003,
        Url = "localhost"
    },
    new ClusterNode
    {
        Port = 4004,
        Url = "localhost"
    }
});
var secondNode = BuildNodeHost(new ConcurrentBag<PeerInfo>
{
    new PeerInfo { TermId = "termId", TermIndex = 0 }
}, 4002, new ConcurrentBag<ClusterNode>
{
    new ClusterNode
    {
        Port = 4001,
        Url = "localhost"
    },
    new ClusterNode
    {
        Port = 4003,
        Url = "localhost"
    },
    new ClusterNode
    {
        Port = 4004,
        Url = "localhost"
    }
});
var thirdNode = BuildNodeHost(new ConcurrentBag<PeerInfo>
{
    new PeerInfo { TermId = "termId", TermIndex = 0 }
}, 4003, new ConcurrentBag<ClusterNode>
{
    new ClusterNode
    {
        Port = 4001,
        Url = "localhost"
    },
    new ClusterNode
    {
        Port = 4002,
        Url = "localhost"
    },
    new ClusterNode
    {
        Port = 4004,
        Url = "localhost"
    }
});
var fourthNode = BuildNodeHost(new ConcurrentBag<PeerInfo>
{
    new PeerInfo { TermId = "termId", TermIndex = 0 }
}, 4004, new ConcurrentBag<ClusterNode>
{
    new ClusterNode
    {
        Port = 4001,
        Url = "localhost"
    },
    new ClusterNode
    {
        Port = 4002,
        Url = "localhost"
    },
    new ClusterNode
    {
        Port = 4003,
        Url = "localhost"
    }
});

// Four different nodes.
// Each node contains one partition.
await firstNode.Start(CancellationToken.None);
await secondNode.Start(CancellationToken.None);
await thirdNode.Start(CancellationToken.None);
await fourthNode.Start(CancellationToken.None);

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