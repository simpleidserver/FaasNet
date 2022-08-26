using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class InstallSnapshotResult : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.INSTALL_SNAPSHOT_RESULT;
        /// <summary>
        /// CurrentTerm, for leader to update itself.
        /// </summary>
        public long Term { get; set; }
        /// <summary>
        /// CurrentIndex
        /// </summary>
        public long MatchIndex { get; set; }
        /// <summary>
        /// True
        /// </summary>
        public bool Success { get; set; }

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
