using FaasNet.EventMesh.Plugin;

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

        [PluginEntryOptionProperty("port", "AMQP Port")]
        public int Port { get; set; }
        /// <summary>
        /// Largest frame size that the sending peer is able to accept on this connection.
        /// </summary>
        [PluginEntryOptionProperty("maxFrameSize", "Largest frame size that the sending peer is able to accept on this connection.")]
        public uint MaxFrameSize { get; set; }
        /// <summary>
        /// The channel-max value is the highest channel number that can be used on the connection.
        /// This value plus one is the maximum number of sessions that can simultaneously active on the connection.
        /// </summary>
        [PluginEntryOptionProperty("maxChannel", "The channel-max value is the highest channel number that can be used on the connection.")]
        public ushort MaxChannel { get; set; }
        /// <summary>
        /// Current maximum number of messages that can be handled at the receiver endpoint of the link.
        /// </summary>
        [PluginEntryOptionProperty("sessionLinkCredit", "Current maximum number of messages that can be handled at the receiver endpoint of the link.")]
        public uint SessionLinkCredit { get; set; }
        /// <summary>
        /// EventMesh server VPN.
        /// </summary>
        [PluginEntryOptionProperty("eventMeshVpn", "EventMesh server VPN.")]
        public string EventMeshVpn { get; set; }
        /// <summary>
        /// EventMesh server URL.
        /// </summary>
        [PluginEntryOptionProperty("eventMeshUrl", "EventMesh server URL.")]
        public string EventMeshUrl { get; set; }
        /// <summary>
        /// EventMesh server port.
        /// </summary>
        [PluginEntryOptionProperty("eventMeshPort", "EventMesh server Port.")]
        public int EventMeshPort { get; set; }
    }
}
