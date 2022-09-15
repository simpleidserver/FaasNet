using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public abstract class BaseConsensusPackage : BasePeerPackage
    {
        public const string MAGIC_CODE = "RaftConsensus";
        public const string PROTOCOL_VERSION = "0000";

        protected BaseConsensusPackage() { }

        public override string MagicCode => MAGIC_CODE;
        public override string VersionNumber => PROTOCOL_VERSION;
        public abstract ConsensusCommands Command { get; }

        public override void SerializeBody(WriteBufferContext context)
        {
            Command.Serialize(context);
            SerializeAction(context);
        }

        protected abstract void SerializeAction(WriteBufferContext context);

        public static BaseConsensusPackage Deserialize(ReadBufferContext context, bool ignoreEnvelope = false)
        {
            if(!ignoreEnvelope)
            {
                context.NextString();
                context.NextString();
            }

            var cmd = ConsensusCommands.Deserialize(context);
            if (cmd == ConsensusCommands.VOTE_REQUEST) return new VoteRequest().Extract(context);
            if (cmd == ConsensusCommands.VOTE_RESULT) return new VoteResult().Extract(context);
            if (cmd == ConsensusCommands.APPEND_ENTRIES_REQUEST) return new AppendEntriesRequest().Extract(context);
            if (cmd == ConsensusCommands.APPEND_ENTRIES_RESULT) return new AppendEntriesResult().Extract(context);
            if (cmd == ConsensusCommands.APPEND_ENTRY_REQUEST) return new AppendEntryRequest().Extract(context);
            if (cmd == ConsensusCommands.APPEND_ENTRY_RESULT) return new AppendEntryResult().Extract(context);
            if(cmd == ConsensusCommands.GET_PEER_STATE_REQUEST) return new GetPeerStateRequest();
            if (cmd == ConsensusCommands.GET_PEER_STATE_RESULT)return new GetPeerStateResult().Extract(context);
            if (cmd == ConsensusCommands.GET_LOGS_REQUEST) return new GetLogsRequest().Extract(context);
            if (cmd == ConsensusCommands.GET_LOGS_RESULT) return new GetLogsResult().Extract(context);
            if (cmd == ConsensusCommands.INSTALL_SNAPSHOT_REQUEST) return new InstallSnapshotRequest().Extract(context);
            if (cmd == ConsensusCommands.INSTALL_SNAPSHOT_RESULT) return new InstallSnapshotResult().Extract(context);
            if (cmd == ConsensusCommands.QUERY_REQUEST) return new QueryRequest().Extract(context);
            if (cmd == ConsensusCommands.QUERY_RESULT) return new QueryResult().Extract(context);
            return null;
        }
    }
}
