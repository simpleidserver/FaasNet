using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class InstallSnapshotRequest : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.INSTALL_SNAPSHOT_REQUEST;

        /// <summary>
        /// So follower can redirect clients.
        /// </summary>
        public string LeaderId { get; set; }

        /// <summary>
        /// Term of the snapshot.
        /// </summary>
        public long SnapshotTerm { get; set; }

        /// <summary>
        /// Index of the snapshot.
        /// </summary>
        public long SnapshotIndex { get; set; }

        /// <summary>
        /// Is finished
        /// </summary>
        public bool IsFinished { get; set; }

        /// <summary>
        /// Index of the record.
        /// </summary>
        public int RecordIndex { get; set; }
        /// <summary>
        /// Is initial request.
        /// </summary>
        public bool IsInit { get; set; } = false;

        /// <summary>
        /// Data of the snapshot.
        /// </summary>
        public IEnumerable<byte> Data { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(LeaderId);
            context.WriteLong(SnapshotTerm);
            context.WriteLong(SnapshotIndex);
            context.WriteBoolean(IsFinished);
            if (IsFinished) return;
            context.WriteInteger(RecordIndex);
            context.WriteBoolean(IsInit);
            context.WriteByteArray(Data.ToArray());
        }

        public InstallSnapshotRequest Extract(ReadBufferContext context)
        {
            LeaderId = context.NextString();
            SnapshotTerm = context.NextLong();
            SnapshotIndex = context.NextLong();
            IsFinished = context.NextBoolean();
            if (IsFinished) return this;
            RecordIndex = context.NextInt();
            IsInit = context.NextBoolean();
            Data = context.NextByteArray();
            return this;
        }
    }
}
