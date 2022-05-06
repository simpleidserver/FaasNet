namespace FaasNet.RaftConsensus.Core
{
    public class ConsensusNodeOptions
    {
        public ConsensusNodeOptions()
        {
            SynchronizeTimerMS = 2000;
        }

        public int SynchronizeTimerMS { get; set; }
    }
}
