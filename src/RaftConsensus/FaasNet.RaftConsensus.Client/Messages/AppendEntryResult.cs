using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class AppendEntryResult : BaseConsensusPackage
    {
        public long Term { get; set; }
        public long MatchIndex { get; set; }
        public bool Success { get; set; }

        public override ConsensusCommands Command => ConsensusCommands.APPEND_ENTRY_RESULT;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Term);
            context.WriteLong(MatchIndex);
            context.WriteBoolean(Success);
        }

        public void Extract(ReadBufferContext context)
        {
            Term = context.NextLong();
            MatchIndex = context.NextLong();
            Success = context.NextBoolean();
        }
    }
}
