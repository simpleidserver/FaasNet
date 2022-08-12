namespace FaasNet.Peer
{
    public class PeerOptions
    {
        public PeerOptions()
        {
            Url = "localhost";
            Port = 5001;
        }

        /// <summary>
        /// Url of the Peer.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Port of the Peer.
        /// </summary>
        public int Port { get; set; }

        public string Id
        {
            get
            {
                return PeerId.Build(Url, Port).Serialize();
            }
        }
    }
}
