using System;

namespace FaasNet.RaftConsensus.Core
{
    public class ConsensusPeerOptions
    {
        public ConsensusPeerOptions()
        {
            Url = "localhost";
            Port = Constants.DefaultPort;
            CheckLeaderHeartbeatIntervalMS = new Interval { MinMS = 4000, MaxMS = 8000 };
            LeaderHeartbeatExpirationDurationMS = 12000;
            ElectionCheckDurationMS = 1000;
            CheckElectionTimerMS = 200;
            CheckLeaderHeartbeatTimerMS = 200;
            LeaderHeartbeatTimerMS = 1000;
            GossipTimerMS = 1000;
            GossipMaxNodeBroadcast = 2;
            GossipTimeoutHeartbeatMS = 2000;
            GossipClusterNodeDeactivationDurationMS = 3000;
        }

        /// <summary>
        /// Default URL.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Default port.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Interval - Check heartbeat.
        /// </summary>
        public Interval CheckLeaderHeartbeatIntervalMS { get; set; }
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
        /// <summary>
        /// Interval - Send gossip heartbeat.
        /// </summary>
        public int GossipTimerMS { get; set; }
        /// <summary>
        /// Gossip - Maximum number of nodes to broadcast the message.
        /// </summary>
        public int GossipMaxNodeBroadcast{ get; set; }
        /// <summary>
        /// Gossip - Heartbeat request expire after MS.
        /// </summary>
        public int GossipTimeoutHeartbeatMS { get; set; }
        /// <summary>
        /// Gossip - When cluster node is not reachable then deactivate the node.
        /// </summary>
        public int GossipClusterNodeDeactivationDurationMS { get; set; }
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
