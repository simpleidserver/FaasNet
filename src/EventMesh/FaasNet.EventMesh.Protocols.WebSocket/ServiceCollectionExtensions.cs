using FaasNet.EventMesh.Protocols;
using FaasNet.EventMesh.Protocols.WebSocket;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWebSocket(this IServiceCollection services, Action<EventMeshWebSocketOptions> callback = null)
        {
            if (callback == null) services.Configure<EventMeshWebSocketOptions>((o) => { });
            services.Configure(callback);
            services.AddTransient<IProxy, WebSocketProxy>();
            return services;
        }
    }
}
