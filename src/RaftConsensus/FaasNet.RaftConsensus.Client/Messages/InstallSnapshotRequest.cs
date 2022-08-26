using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class InstallSnapshotRequest : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.INSTALL_SNAPSHOT_REQUEST;

        /// <summary>
        /// Leader's term.
        /// </summary>
        public long Term { get; set; }

        /// <summary>
        /// So follower can redirect clients.
        /// </summary>
        public string LeaderId { get; set; }

        /// <summary>
        /// Commit snapshot index.
        /// </summary>
        public long CommitIndex { get; set; }

        /// <summary>
        /// Term of the snapshot.
        /// </summary>
        public long SnapshotTerm { get; set; }

        /// <summary>
        /// Index of the snapshot.
        /// </summary>
        public long SnapshotIndex { get; set; }

        /// <summary>
        /// Data of the snapshot.
        /// </summary>
        public byte[] Data { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Term);
            context.WriteString(LeaderId);
            context.WriteLong(CommitIndex);
            context.WriteLong(SnapshotTerm);
            context.WriteLong(SnapshotIndex);
            context.WriteByteArray(Data);
        }

        public void Extract(ReadBufferContext context)
        {
            Term = context.NextLong();
            LeaderId = context.NextString();
            CommitIndex = context.NextLong();
            SnapshotTerm = context.NextLong();
            SnapshotIndex = context.NextLong();
            Data = context.NextByteArray();
        }
    }
}
