namespace FaasNet.DHT.Kademlia.Core
{
    public class DHTOptions
    {
        public DHTOptions()
        {
            S = 6;
            K = 4;
        }

        /// <summary>
        /// Identifier space.
        /// </summary>
        public int S { get; set; }
        /// <summary>
        /// Maximum number of peers represented in a k-bucket.
        /// </summary>
        public int K {  get; set; }
    }
}
