namespace FaasNet.RaftConsensus.Client.Messages
{
    public class VoteRequest : ConsensusPackage
    {
        public int Term { get; set; }
        public int CandidateId { get; set; }
        public int LastLogIndex { get; set; }
        public int LastLogTerm { get; set; }
    }
}
