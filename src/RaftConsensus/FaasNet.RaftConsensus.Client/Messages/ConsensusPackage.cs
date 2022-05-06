namespace FaasNet.RaftConsensus.Client.Messages
{
    public class ConsensusPackage
    {
        private const string MAGIC_CODE = "RaftConsensus";
        private const string PROTOCOL_VERSION = "0000";

        protected ConsensusPackage() { }

        public ConsensusHeader Header { get; set; }

        public virtual void Serialize(WriteBufferContext context)
        {
            context.WriteString(MAGIC_CODE);
            context.WriteString(PROTOCOL_VERSION);
            Header.Serialize(context);
        }

        public static ConsensusPackage Deserialize(ReadBufferContext context)
        {
            var magicCode = context.NextString();
            var version = context.NextString();
            if (magicCode != MAGIC_CODE || version != PROTOCOL_VERSION) return null;
            var header = ConsensusHeader.Deserialize(context);
            if (header.Command == ConsensusCommands.VOTE_REQUEST) return new VoteRequest { Header = header };
            if (header.Command == ConsensusCommands.LEADER_HEARTBEAT_REQUEST) return new LeaderHeartbeatRequest { Header = header };
            if (header.Command == ConsensusCommands.LEADER_HEARTBEAT_RESULT) return new LeaderHeartbeatResult { Header = header };
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

            return new ConsensusPackage
            {
                Header = header
            };
        }
    }
}
