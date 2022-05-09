namespace FaasNet.RaftConsensus.Client.Messages.Gossip
{
    public class GossipPackage
    {
        private const string MAGIC_CODE = "Gossip";
        private const string PROTOCOL_VERSION = "0000";

        public GossipHeader Header { get; set; }

        public virtual void Serialize(WriteBufferContext context)
        {
            context.WriteString(MAGIC_CODE);
            context.WriteString(PROTOCOL_VERSION);
            Header.Serialize(context);
        }

        public static GossipPackage Deserialize(ReadBufferContext context)
        {
            var magicCode = context.NextString();
            var version = context.NextString();
            if (version != PROTOCOL_VERSION || magicCode != MAGIC_CODE) return null;
            var header = GossipHeader.Deserialize(context);
            if (header.Command == GossipCommands.HEARTBEAT_REQUEST) return new GossipHeartbeatRequest { Header = header };
            if(header.Command == GossipCommands.HEARTBEAT_RESULT)
            {
                var result = new GossipHeartbeatResult { Header = header };
                result.Extract(context);
                return result;
            }

            if(header.Command == GossipCommands.SYNC_REQUEST)
            {
                var result = new GossipSyncStateRequest { Header = header };
                result.Extract(context);
                return result;
            }

            if (header.Command == GossipCommands.SYNC_RESULT)
            {
                var result = new GossipSyncStateResult { Header = header };
                result.Extract(context);
                return result;
            }

            if (header.Command == GossipCommands.UPDATE_NODE_STATE_REQUEST)
            {
                var result = new GossipUpdateNodeStateRequest { Header = header };
                result.Extract(context);
                return result;
            }

            if (header.Command == GossipCommands.JOIN_NODE_REQUEST)
            {
                var result = new GossipJoinNodeRequest { Header = header };
                result.Extract(context);
                return result;
            }

            return new GossipPackage { Header = header };
        }
    }
}
