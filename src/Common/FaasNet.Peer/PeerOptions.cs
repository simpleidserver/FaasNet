namespace FaasNet.Peer
{
    public class PeerOptions
    {
        public PeerOptions()
        {
            Url = "localhost";
            Port = 5001;
            IsSelfRegistrationEnabled = true;
        }

        /// <summary>
        /// Url of the Peer.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Port of the Peer.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Enable or disable self registration.
        /// </summary>
        public bool IsSelfRegistrationEnabled { get; set; }
        /// <summary>
        /// Key of the partition.
        /// </summary>
        public string PartitionKey { get; set; }

        public string Id
        {
            get
            {
                return PeerId.Build(Url, Port).Serialize();
            }
        }
    }
}
