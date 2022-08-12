using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class AppendEntriesResult : BaseConsensusPackage
    {
        /// <summary>
        /// CurrentTerm, for leader to update itself.
        /// </summary>
        public long Term { get; set; }
        /// <summary>
        /// Curre
        /// ntIndex
        /// </summary>
        public long MatchIndex { get; set; }
        /// <summary>
        /// True if follower contained entry matching prevLogIndex and prevLogTerm.
        /// </summary>
        public bool Success { get; set; }

        public override ConsensusCommands Command => ConsensusCommands.APPEND_ENTRIES_RESULT;

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
