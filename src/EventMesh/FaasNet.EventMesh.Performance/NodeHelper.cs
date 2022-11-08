using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Core;
using System.Collections.Concurrent;

namespace FaasNet.EventMesh.Performance
{
    public static class NodeHelper
    { 
        public static async Task<IPeerHost> BuildAndStartNode(int port, ConcurrentBag<ClusterPeer> clusterNodes, int startPeerPort = 30000, Action<string> leaderCallback = null)
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
                    });
                };
            }, clusterNodes: clusterNodes)
                .UseUDPTransport()
                .UseEventMesh(o =>
                {
                });
            var node = nodeHostFactory.Build();
            await node.Start();
            return node;
        }
    }
}
