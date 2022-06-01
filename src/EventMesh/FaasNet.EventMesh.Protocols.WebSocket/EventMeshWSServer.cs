using NetCoreServer;
using System.Net;

namespace FaasNet.EventMesh.Protocols.WebSocket
{
    public class EventMeshWSServer : WsServer
    {
        private readonly EventMeshWebSocketOptions _options;

        public EventMeshWSServer(EventMeshWebSocketOptions options, IPAddress address, int port) : base(address, port) 
        { 
            _options = options;
        }

        protected override TcpSession CreateSession() { return new EventMeshServerWSSession(_options, this); }
    }
}
