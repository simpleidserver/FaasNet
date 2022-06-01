using FaasNet.EventMesh.Protocols.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseWebSocket(this ServerBuilder serverBuilder, Action<EventMeshWebSocketOptions> callback = null)
        {
            serverBuilder.Services.AddWebSocket(callback);
            return serverBuilder;
        }
    }
}
