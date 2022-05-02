namespace FaasNet.RaftConsensus.Core
{
    public class ConsensusPeerOptions
    {
        public ConsensusPeerOptions()
        {
            Port = Constants.DefaultPort;
            LeaderHeartbeatDurationMS = 5 * 1000;
            ElectionCheckDurationMS = 5 * 1000;
            CheckElectionTimerMS = 200;
            LeaderHeartbeatTimerMS = 1000;
        }

        /// <summary>
        /// Default port.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Expiration time MS - Heartbeat check.
        /// </summary>
        public int LeaderHeartbeatDurationMS { get; set; }
        /// <summary>
        /// Expiration time MS - election check.
        /// </summary>
        public int ElectionCheckDurationMS { get; set; }
        /// <summary>
        /// Interval - Check election result.
        /// </summary>
        public int CheckElectionTimerMS { get; set; }
        /// <summary>
        /// Interval - Send heartbeat.
        /// </summary>
        public int LeaderHeartbeatTimerMS { get; set; }
    }
}
