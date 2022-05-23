using FaasNet.EventMesh.Protocols.AMQP;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseAMQPProtocol(this ServerBuilder serverBuilder, Action<EventMeshAMQPOptions> callback)
        {
            serverBuilder.Services.AddAMQPProtocol(callback);
            return serverBuilder;
        }
    }
}
