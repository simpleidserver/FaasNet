using System.Collections.Generic;
using System.Linq;

namespace FaasNet.RaftConsensus.Client.Messages.Gossip
{
    public class GossipPackageResultBuilder
    {
        public static GossipPackage Heartbeat(string sourceUrl, int sourcePort, Dictionary<string, int> states)
        {
            return new GossipHeartbeatResult
            {
                Header = new GossipHeader(GossipCommands.HEARTBEAT_RESULT, sourceUrl, sourcePort),
                States = states.Select(kvp => new GossipState { EntityType = kvp.Key, EntityVersion = kvp.Value }).ToList()
            };
        }

        public static GossipPackage Sync(string sourceUrl, int sourcePort, Dictionary<string, (int Version, string Value)> states)
        {
            return new GossipSyncStateResult
            {
                Header = new GossipHeader(GossipCommands.SYNC_RESULT, sourceUrl, sourcePort),
                States = states.Select(kvp => new GossipStateResult { EntityType = kvp.Key, EntityVersion = kvp.Value.Version, Value = kvp.Value.Value }).ToList()
            };
        }
    }
}
