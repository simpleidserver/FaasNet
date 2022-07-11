namespace FaasNet.Peer
{
    public class PeerOptions
    {
        public PeerOptions()
        {
            PeerId = "peer";
        }

        /// <summary>
        /// Unique identifier of the peer.
        /// </summary>
        public string PeerId { get; set; }
        /// <summary>
        /// Url of the Peer.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Port of the Peer.
        /// </summary>
        public int Port { get; set; }
    }
}
