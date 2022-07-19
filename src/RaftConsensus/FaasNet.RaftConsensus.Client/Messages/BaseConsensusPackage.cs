using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public abstract class BaseConsensusPackage : BasePeerPackage
    {
        public const string MAGIC_CODE = "RaftConsensus";
        public const string PROTOCOL_VERSION = "0000";

        protected BaseConsensusPackage() { }

        public ConsensusHeader Header { get; set; }

        public override string MagicCode => MAGIC_CODE;
        public override string VersionNumber => PROTOCOL_VERSION;

        public override void SerializeBody(WriteBufferContext context)
        {
            Header.Serialize(context);
            SerializeAction(context);
        }

        public abstract void SerializeAction(WriteBufferContext context);

        public static BaseConsensusPackage Deserialize(ReadBufferContext context, bool ignoreEnvelope = false)
        {
            if(!ignoreEnvelope)
            {
                context.NextString();
                context.NextString();
            }

            var header = ConsensusHeader.Deserialize(context);
            if (header.Command == ConsensusCommands.EMPTY_RESULT) return new EmptyConsensusPackage { Header = header };
            if (header.Command == ConsensusCommands.VOTE_REQUEST) return new VoteRequest { Header = header };
            if (header.Command == ConsensusCommands.LEADER_HEARTBEAT_REQUEST) return new LeaderHeartbeatRequest { Header = header };
            if (header.Command == ConsensusCommands.LEADER_HEARTBEAT_RESULT) return new LeaderHeartbeatResult { Header = header };
            if (header.Command == ConsensusCommands.GET_REQUEST) return new GetEntryRequest { Header = header };
            if (header.Command == ConsensusCommands.VOTE_RESULT)
            {
                var result = new VoteResult
                {
                    Header = header
                };
                result.Extract(context);
                return result;
            }

            if(header.Command == ConsensusCommands.APPEND_ENTRY_REQUEST)
            {
                var result = new AppendEntryRequest { Header = header };
                result.Extract(context);
                return result;
            }

            if (header.Command == ConsensusCommands.GET_RESULT)
            {
                var result = new GetEntryResult { Header = header };
                result.Extract(context);
                return result;
            }

            return null;
        }
    }
}
