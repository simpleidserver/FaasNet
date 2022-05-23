namespace FaasNet.RaftConsensus.Core
{
    public class ConsensusNodeOptions
    {
        public ConsensusNodeOptions()
        {
            SynchronizeTimerMS = 2000;
            GossipTimerMS = 1000;
            GossipMaxNodeBroadcast = 2;
            GossipTimeoutHeartbeatMS = 2000;
            GossipClusterNodeDeactivationDurationMS = 3000;
            Port = 4000;
            Url = "localhost";
        }

        public int SynchronizeTimerMS { get; set; }
        /// <summary>
        /// Interval - Send gossip heartbeat.
        /// </summary>
        public int GossipTimerMS { get; set; }
        /// <summary>
        /// Gossip - Maximum number of nodes to broadcast the message.
        /// </summary>
        public int GossipMaxNodeBroadcast { get; set; }
        /// <summary>
        /// Gossip - Heartbeat request expire after MS.
        /// </summary>
        public int GossipTimeoutHeartbeatMS { get; set; }
        /// <summary>
        /// Gossip - When cluster node is not reachable then deactivate the node.
        /// </summary>
        public int GossipClusterNodeDeactivationDurationMS { get; set; }
        public string Url { get; set; }
        public int Port { get; set; }
    }
}
