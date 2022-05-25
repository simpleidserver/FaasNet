namespace FaasNet.EventMesh.Protocols.AMQP
{
    public class EventMeshAMQPOptions
    {
        public EventMeshAMQPOptions()
        {
            Port = 5672;
            MaxFrameSize = 1000;
            MaxChannel = 1000;
            SessionLinkCredit = 255;
            EventMeshVpn = Client.Constants.DefaultVpn;
            EventMeshUrl = Client.Constants.DefaultUrl;
            EventMeshPort = Client.Constants.DefaultPort;
        }

        public int Port { get; set; }
        /// <summary>
        /// Largest frame size that the sending peer is able to accept on this connection.
        /// </summary>
        public uint MaxFrameSize { get; set; }
        /// <summary>
        /// The channel-max value is the highest channel number that can be used on the connection.
        /// This value plus one is the maximum number of sessions that can simultaneously active on the connection.
        /// </summary>
        public ushort MaxChannel { get; set; }
        /// <summary>
        /// Current maximum number of messages that can be handled at the receiver endpoint of the link.
        /// </summary>
        public uint SessionLinkCredit { get; set; }
        public string EventMeshVpn { get; set; }
        public string EventMeshUrl { get; set; }
        public int EventMeshPort { get; set; }
    }
}
