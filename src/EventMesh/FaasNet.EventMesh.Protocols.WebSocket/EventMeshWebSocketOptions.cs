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
        public int Port { get; set; }
        /// <summary>
        /// URL of the eventmesh server.
        /// </summary>
        public string EventMeshUrl { get; set; }
        /// <summary>
        /// Port of the eventmesh server.
        /// </summary>
        public int EventMeshPort { get; set; }
    }
}
