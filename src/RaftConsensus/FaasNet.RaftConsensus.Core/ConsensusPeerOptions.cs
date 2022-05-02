namespace FaasNet.RaftConsensus.Core
{
    public class ConsensusPeerOptions
    {
        public ConsensusPeerOptions()
        {
            Port = Constants.DefaultPort;
            LeaderHeartbeatDurationMS = 5 * 1000;
            ElectionCheckDurationMS = 5 * 1000;
            TimerMS = 200;
        }

        public int Port { get; set; }
        public int LeaderHeartbeatDurationMS { get; set; }
        public int ElectionCheckDurationMS { get; set; }
        public int TimerMS { get; set; }
    }
}
