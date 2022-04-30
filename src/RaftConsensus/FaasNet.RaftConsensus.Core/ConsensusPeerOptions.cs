namespace FaasNet.RaftConsensus.Core
{
    public class ConsensusPeerOptions
    {
        public ConsensusPeerOptions()
        {
            Port = Constants.DefaultPort;
            LeaderHeartbeatDurationMS = 5 * 1000;
            CheckLeaderHeartbeatTimerMS = 200;
        }

        public int Port { get; set; }
        public int LeaderHeartbeatDurationMS { get; set; }
        public int CheckLeaderHeartbeatTimerMS { get; set; }
    }
}
