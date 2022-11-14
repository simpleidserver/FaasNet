using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FaasNet.EventMesh.Performance
{
    public static class NodeHelper
    { 
        public static async Task<IPeerHost> BuildAndStartNode(int port, ConcurrentBag<ClusterPeer> clusterNodes, int startPeerPort = 30000, Action<string> leaderCallback = null, Action<string> followerCallback = null)
        {
            PartitionedNodeHostFactory nodeHostFactory = PartitionedNodeHostFactory.New(options: p =>
            {
                p.Port = port;
            }, nodeOptions: no =>
            {
                no.StartPeerPort = startPeerPort;
                no.CallbackPeerConfiguration = (p) =>
                {
                    p.Services.PostConfigure<RaftConsensusPeerOptions>(o =>
                    {
                        o.LeaderCallback += leaderCallback;
                        o.FollowerCallback += followerCallback;
                    });
                };
                no.CallbackPeerDependencies = (s) =>
                {
                    s.AddLogging(l =>
                    {
                        l.ClearProviders();
                        l.AddConsole();
                        l.SetMinimumLevel(LogLevel.Information);
                    });
                };
            }, clusterNodes: clusterNodes)
                .UseUDPTransport()
                .UseEventMesh();
            var node = nodeHostFactory.Build();
            await node.Start();
            return node;
        }
    }
}
