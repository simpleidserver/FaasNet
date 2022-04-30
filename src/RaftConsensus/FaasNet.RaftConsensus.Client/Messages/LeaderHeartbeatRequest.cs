namespace FaasNet.RaftConsensus.Client.Messages
{
    public class LeaderHeartbeatRequest : ConsensusPackage
    {
        public string TermId { get; set; }
        public long Index { get; set; }
    }
}
