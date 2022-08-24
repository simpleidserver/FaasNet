namespace FaasNet.DHT.Kademlia.Core
{
    public class KademliaOptions
    {
        /// <summary>
        /// Identifier space.
        /// </summary>
        public int S { get; set; } = 6;
        /// <summary>
        /// Maximum number of peers represented in a k-bucket.
        /// </summary>
        public int K { get; set; } = 4;
        /// <summary>
        /// Interval in MS used to refresh the KBUCKET list.
        /// </summary>
        public double FixKBucketLstTimerMS { get; set; } = 500;
        /// <summary>
        /// Interval in MS used to check the peers.
        /// </summary>
        public double HealthCheckTimerMS { get; set; } = 500;
        /// <summary>
        /// Is a Seed peer.
        /// </summary>
        public bool IsSeedPeer { get; set; }
        /// <summary>
        /// Url of the Seed peer.
        /// </summary>
        public string SeedUrl { get; set; }
        /// <summary>
        /// Port of the Seed peer.
        /// </summary>
        public int SeedPort { get; set; }
        /// <summary>
        /// Identifier of the Peer.
        /// </summary>
        public long KademliaPeerId { get; set; } = 1;
        /// <summary>
        /// Timeout of a request in MS
        /// </summary>
        public int RequestTimeoutMS { get; set; } = 5000;
    }
}
