using System;

namespace FaasNet.RaftConsensus.Core
{
    public class RaftConsensusPeerOptions
    {
        public RaftConsensusPeerOptions()
        {
            ElectionTimer = new Interval(1000, 10000);
            LeaderHeartbeatTimerMS = 2000;
            LeaderHeartbeatExpirationDurationMS = 12000;
        }

        /// <summary>
        /// Random election timer in MS.
        /// </summary>
        public Interval ElectionTimer { get; set; }
        /// <summary>
        /// Interval in MS used by the leader to send Heartbeat.
        /// </summary>
        public int LeaderHeartbeatTimerMS { get; set; }
        /// <summary>s
        /// Sliding expiration time of the heartbeat.
        /// </summary>
        public int LeaderHeartbeatExpirationDurationMS { get; set; }
        /// <summary>
        /// Configuration's folder location.
        /// </summary>
        public string ConfigurationDirectoryPath { get; set; }
    }

    public class Interval
    {
        public Interval(int minMS, int maxMS)
        {
            MinMS = minMS;
            MaxMS = maxMS;
        }

        public int MinMS { get; private set; }
        public int MaxMS { get; private set; }

        public int GetValue()
        {
            return new Random().Next(MinMS, MaxMS);
        }
    }
}
