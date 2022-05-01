namespace FaasNet.RaftConsensus.Core.Models
{
    public class PeerInfo
    {
        public string TermId { get; set; }
        public long TermIndex { get; set; }
    }
}
