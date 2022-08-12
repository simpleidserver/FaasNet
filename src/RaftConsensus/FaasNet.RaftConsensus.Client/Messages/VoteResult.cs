using FaasNet.Peer.Client;
using System;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class VoteResult : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.VOTE_RESULT;

        /// <summary>
        /// CurrentTerm, for candidate to update itself.
        /// </summary>
        public long Term { get; set; }
        /// <summary>
        /// True eans candidate received vote.
        /// </summary>
        public bool VoteGranted { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Term);
            context.WriteBoolean(VoteGranted);
        }

        public void Extract(ReadBufferContext context)
        {
            Term = context.NextLong();
            VoteGranted = context.NextBoolean();
        }
    }
}
