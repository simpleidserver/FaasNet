namespace FaasNet.DHT.Kademlia.Core
{
    public class DHTOptions
    {
        public DHTOptions()
        {
            S = 6;
            K = 4;
            FixKBucketLstTimerMS = 500;
            HealthCheckTimerMS = 500;
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
    }
}
