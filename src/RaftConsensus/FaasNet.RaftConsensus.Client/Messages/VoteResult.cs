namespace FaasNet.RaftConsensus.Client.Messages
{
    public class VoteResult : ConsensusPackage
    {
        public int Term { get; set; }
        public bool VoteGranted { get; set; }
    }
}
