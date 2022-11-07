using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using System.Collections.Concurrent;

namespace FaasNet.EventMesh.Performance
{
    public static class NodeHelper
    { 
        public static async Task<IPeerHost> BuildAndStartNode(int port, ConcurrentBag<ClusterPeer> clusterNodes, int startPeerPort = 30000)
        {
            PartitionedNodeHostFactory nodeHostFactory = PartitionedNodeHostFactory.New(options: p =>
            {
                p.Port = port;
            }, nodeOptions: no =>
            {
                no.StartPeerPort = startPeerPort;
            }, clusterNodes: clusterNodes)
                .UseUDPTransport()
                .UseEventMesh();
            var node = nodeHostFactory.Build();
            await node.Start();
            return node;
        }
    }
}
