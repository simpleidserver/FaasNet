using System.Collections.Generic;
using System.Linq;

namespace FaasNet.RaftConsensus.Client.Messages.Gossip
{
    public static class GossipPackageRequestBuilder
    {
        public static GossipPackage Heartbeat(string sourceUrl, int sourcePort)
        {
            return new GossipHeartbeatRequest
            {
                Header = new GossipHeader(GossipCommands.HEARTBEAT_REQUEST, sourceUrl, sourcePort)
            };
        }

        public static GossipPackage Sync(string sourceUrl, int sourcePort, Dictionary<string, int> states)
        {
            return new GossipSyncStateRequest
            {
                Header = new GossipHeader(GossipCommands.SYNC_REQUEST, sourceUrl, sourcePort),
                States = states.Select(kvp => new GossipState { EntityType = kvp.Key, EntityVersion = kvp.Value }).ToList()
            };
        }

        public static GossipPackage AddNode(string url, int port)
        {
            return new GossipJoinNodeRequest
            {
                Header = new GossipHeader(GossipCommands.JOIN_NODE_REQUEST, url, port),
                Port = port,
                Url = url
            };
        }

        public static GossipPackage UpdateClusterNodes(string url, int port, ICollection<ClusterNodeMessage> clusterNodes)
        {
            return new GossipUpdateClusterRequest
            {
                Header = new GossipHeader(GossipCommands.UPDATE_CLUSTER_NODES_REQUEST, url, port),
                Nodes = clusterNodes
            };
        }
    }
}
