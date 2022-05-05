using System;

namespace FaasNet.RaftConsensus.Core
{
    public class PeerHostEventArgs : EventArgs
    {
        public PeerHostEventArgs(string termId, string nodeId, string peerId)
        {
            TermId = termId;
            NodeId = nodeId;
            PeerId = peerId;
        }

        public string TermId { get; private set; }
        public string NodeId { get; private set; }
        public string PeerId { get; private set; }
    }
}
