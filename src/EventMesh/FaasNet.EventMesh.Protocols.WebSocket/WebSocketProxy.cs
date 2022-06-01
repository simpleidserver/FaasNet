using Microsoft.Extensions.Options;
using System.Net;

namespace FaasNet.EventMesh.Protocols.WebSocket
{
    public class WebSocketProxy : BaseProxy
    {
        private readonly EventMeshWebSocketOptions _options;
        private EventMeshWSServer _wsServer;

        public WebSocketProxy(IOptions<EventMeshWebSocketOptions> options)
        {
            _options = options.Value;
        }

        protected override void Init()
        {
            _wsServer = new EventMeshWSServer(_options, IPAddress.Any, _options.Port);
            _wsServer.Start();
        }

        protected override void Shutdown()
        {
            _wsServer.Stop();
        }
    }
}
