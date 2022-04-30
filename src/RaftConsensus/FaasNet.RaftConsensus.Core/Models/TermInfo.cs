using System;

namespace FaasNet.RaftConsensus.Core.Models
{
    public class TermInfo
    {
        public string Id { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public DateTime LastHeartbeatRequest { get; set; }
        public long Index { get; set; }
        public PeerStates State { get; set; }
    }
}
