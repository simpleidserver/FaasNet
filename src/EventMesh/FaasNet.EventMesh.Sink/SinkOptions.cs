using FaasNet.EventMesh.Plugin;

namespace FaasNet.EventMesh.Sink
{
    public class SinkOptions
    {
        public SinkOptions()
        {
            EventMeshUrl = "localhost";
            EventMeshPort = 4000;
            Vpn = "default";
            ClientId = "publishClientId";
        }

        /// <summary>
        /// EventMesh server URL.
        /// </summary>
        [PluginEntryOptionProperty("eventMeshUrl", "EventMesh server URL.")]
        public string EventMeshUrl { get; set; }
        /// <summary>
        /// EventMesh server Port.
        /// </summary>
        [PluginEntryOptionProperty("eventMeshPort", "EventMesh server Port.")]
        public int EventMeshPort { get; set; }
        /// <summary>
        /// EventMesh server VPN.
        /// </summary>
        [PluginEntryOptionProperty("eventMeshVpn", "EventMesh server VPN.")]
        public string Vpn { get; set; }
        /// <summary>
        /// Client identifier used to publish message.
        /// </summary>
        [PluginEntryOptionProperty("clientId", "Client identifier used to publish message.")]
        public string ClientId { get; set; }
    }
}
