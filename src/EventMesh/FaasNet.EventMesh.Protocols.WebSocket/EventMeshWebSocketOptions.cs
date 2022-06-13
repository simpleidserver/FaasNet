using FaasNet.EventMesh.Plugin;

namespace FaasNet.EventMesh.Protocols.WebSocket
{
    public class EventMeshWebSocketOptions
    {
        public EventMeshWebSocketOptions()
        {
            Port = 2803;
            EventMeshUrl = "localhost";
            EventMeshPort = 4000;
        }

        /// <summary>
        /// Port of the websocket server.
        /// </summary>
        [PluginEntryOptionProperty("port", "Websocket Port")]
        public int Port { get; set; }
        /// <summary>
        /// URL of the eventmesh server.
        /// </summary>
        [PluginEntryOptionProperty("eventMeshUrl", "EventMesh server URL.")]
        public string EventMeshUrl { get; set; }
        /// <summary>
        /// Port of the eventmesh server.
        /// </summary>
        [PluginEntryOptionProperty("eventMeshPort", "EventMesh server Port.")]
        public int EventMeshPort { get; set; }
    }
}
