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
            if (cmd == ConsensusCommands.VOTE_REQUEST)
            {
                var result = new VoteRequest();
                result.Extract(context);
                return result;
            }

            if (cmd == ConsensusCommands.VOTE_RESULT)
            {
                var result = new VoteResult();
                result.Extract(context);
                return result;
            }

            if (cmd == ConsensusCommands.APPEND_ENTRIES_REQUEST)
            {
                var result = new AppendEntriesRequest();
                result.Extract(context);
                return result;
            }

            if (cmd == ConsensusCommands.APPEND_ENTRIES_RESULT)
            {
                var result = new AppendEntriesResult();
                result.Extract(context);
                return result;
            }

            if (cmd == ConsensusCommands.APPEND_ENTRY_REQUEST)
            {
                var result = new AppendEntryRequest();
                result.Extract(context);
                return result;
            }

            if (cmd == ConsensusCommands.APPEND_ENTRY_RESULT)
            {
                var result = new AppendEntryResult();
                result.Extract(context);
                return result;
            }

            if(cmd == ConsensusCommands.GET_PEER_STATE_REQUEST) return new GetPeerStateRequest();
            if (cmd == ConsensusCommands.GET_PEER_STATE_RESULT)
            {
                var result = new GetPeerStateResult();
                result.Extract(context);
                return result;
            }

            if (cmd == ConsensusCommands.GET_LOGS_REQUEST)
            {
                var result = new GetLogsRequest();
                result.Extract(context);
                return result;
            }

            if (cmd == ConsensusCommands.GET_LOGS_RESULT)
            {
                var result = new GetLogsResult();
                result.Extract(context);
                return result;
            }

            if (cmd == ConsensusCommands.INSTALL_SNAPSHOT_REQUEST)
            {
                var result = new InstallSnapshotRequest();
                result.Extract(context);
                return result;
            }

            if (cmd == ConsensusCommands.INSTALL_SNAPSHOT_RESULT)
            {
                var result = new InstallSnapshotResult();
                result.Extract(context);
                return result;
            }

            if (cmd == ConsensusCommands.GET_STATEMACHINE_REQUEST) return new GetStateMachineRequest();
            if (cmd == ConsensusCommands.GET_STATEMACHINE_RESULT)
            {
                var result = new GetStateMachineResult();
                result.Extract(context);
                return result;
            }

            return null;
        }
    }
}
