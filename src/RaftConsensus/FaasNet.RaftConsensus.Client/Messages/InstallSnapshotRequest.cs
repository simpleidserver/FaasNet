using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

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
        /// Current iteration.
        /// </summary>
        public int Iteration { get; set; }

        /// <summary>
        /// Total number of iterations.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Data of the snapshot.
        /// </summary>
        public IEnumerable<IEnumerable<byte>> Data { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Term);
            context.WriteString(LeaderId);
            context.WriteLong(CommitIndex);
            context.WriteLong(SnapshotTerm);
            context.WriteLong(SnapshotIndex);
            context.WriteInteger(Iteration);
            context.WriteInteger(Total);
            context.WriteInteger(Data == null ? 0 : Data.Count());
            if (Data != null)
                foreach (var payload in Data)
                    context.WriteByteArray(payload.ToArray());
        }

        public InstallSnapshotRequest Extract(ReadBufferContext context)
        {
            Term = context.NextLong();
            LeaderId = context.NextString();
            CommitIndex = context.NextLong();
            SnapshotTerm = context.NextLong();
            SnapshotIndex = context.NextLong();
            Iteration = context.NextInt();
            Total = context.NextInt();
            int nb = context.NextInt();
            var result = new List<IEnumerable<byte>>();
            for (var i = 0; i < nb; i++) result.Add(context.NextByteArray());
            Data = result;
            return this;
        }
    }
}
