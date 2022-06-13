using FaasNet.EventMesh.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.EventMesh.Protocols.WebSocket
{
    public class EventMeshWebSocketPlugin : IPlugin<EventMeshWebSocketOptions>
    {
        public void Load(IServiceCollection services, EventMeshWebSocketOptions options)
        {
            services.AddWebSocket(o =>
            {
                o.Port = options.Port;
                o.EventMeshPort = options.EventMeshPort;
                o.EventMeshUrl = options.EventMeshUrl;
            });
        }
    }
}
