using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Messages
{
    public class InstallSnapshotResult : BaseConsensusPackage
    {
        public override ConsensusCommands Command => ConsensusCommands.INSTALL_SNAPSHOT_RESULT;
        /// <summary>
        /// True
        /// </summary>
        public bool Success { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
        }

        public InstallSnapshotResult Extract(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            return this;
        }
    }
}
