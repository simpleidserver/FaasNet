using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class VoteRequest : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.VOTE_REQUEST;

        /// <summary>
        /// Candidate's term.
        /// </summary>
        public long Term { get; set; }
        /// <summary>
        /// Candidate requesting vote.
        /// </summary>
        public string CandidateId { get; set; }
        /// <summary>
        /// Index of candidate's last log entry.
        /// </summary>
        public long LastLogIndex { get; set; }
        /// <summary>
        /// Term of candidate's last log entry.
        /// </summary>
        public long LastLogTerm { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Term);
            context.WriteString(CandidateId);
            context.WriteLong(LastLogIndex);
            context.WriteLong(LastLogTerm);
        }

        public void Extract(ReadBufferContext context)
        {
            Term = context.NextLong();
            CandidateId = context.NextString();
            LastLogIndex = context.NextLong();
            LastLogTerm = context.NextLong();
        }
    }
}
