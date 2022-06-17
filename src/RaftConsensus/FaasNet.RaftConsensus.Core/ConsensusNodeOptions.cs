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
            Url = "localhost";
            Port = 4000;
            ExposedUrl = "localhost";
            ExposedPort = 4000;
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
        /// <summary>
        /// Url of the node.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Port of the node.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Exposed Url.
        /// </summary>
        public string ExposedUrl { get; set; }
        /// <summary>
        /// Exposed Port.
        /// </summary>
        public int ExposedPort { get; set; }
    }
}
