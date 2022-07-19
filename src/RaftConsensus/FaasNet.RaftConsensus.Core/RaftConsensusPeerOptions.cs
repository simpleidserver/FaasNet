using System;

namespace FaasNet.RaftConsensus.Core
{
    public class RaftConsensusPeerOptions
    {
        public RaftConsensusPeerOptions()
        {
            ElectionIntervalMS = new Interval { MinMS = 1000, MaxMS = 10000 };
            LeaderHeartbeatExpirationDurationMS = 12000;
            ElectionCheckDurationMS = 1000;
            CheckElectionTimerMS = 50;
            CheckLeaderHeartbeatTimerMS = 50;
            LeaderHeartbeatTimerMS = 50;
        }


        /// <summary>
        /// Election interval.
        /// </summary>
        public Interval ElectionIntervalMS { get; set; }
        /// <summary>
        /// Expiration - Heartbeat.
        /// </summary>
        public int LeaderHeartbeatExpirationDurationMS { get; set; }
        /// <summary>
        /// Expiration time MS - election check.
        /// </summary>
        public int ElectionCheckDurationMS { get; set; }
        /// <summary>
        /// Interval - Check election result.
        /// </summary>
        public int CheckElectionTimerMS { get; set; }
        /// <summary>
        /// Interval - Check hearbeat timer.
        /// </summary>
        public int CheckLeaderHeartbeatTimerMS { get; set; }
        /// <summary>
        /// Interval - Send heartbeat.
        /// </summary>
        public int LeaderHeartbeatTimerMS { get; set; }
    }

    public class Interval
    {
        public int MinMS { get; set; }
        public int MaxMS { get; set; }

        public int GetValue()
        {
            return new Random().Next(MinMS, MaxMS);
        }
    }
}
