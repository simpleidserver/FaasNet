namespace FaasNet.DHT.Kademlia.Core
{
    public class KademliaOptions
    {
        public KademliaOptions()
        {
            S = 6;
            K = 4;
            FixKBucketLstTimerMS = 500;
            HealthCheckTimerMS = 500;
            KademliaPeerId = 1;
        }

        /// <summary>
        /// Identifier space.
        /// </summary>
        public int S { get; set; }
        /// <summary>
        /// Maximum number of peers represented in a k-bucket.
        /// </summary>
        public int K {  get; set; }
        /// <summary>
        /// Interval in MS used to refresh the KBUCKET list.
        /// </summary>
        public double FixKBucketLstTimerMS { get; set; }
        /// <summary>
        /// Interval in MS used to check the peers.
        /// </summary>
        public double HealthCheckTimerMS { get; set; }
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
        public long KademliaPeerId { get; set; }
    }
}
