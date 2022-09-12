using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.UI.ViewModels
{
	public class PeerStateViewModel
	{
		public string Name { get; set; }
        public PeerStatus Status { get; set; }
        public long CommitIndex { get; set; }
        public long LastApplied { get; set; }
        public long SnapshotCommitIndex { get; set; }
        public long SnapshotLastApplied { get; set; }
    }
}
