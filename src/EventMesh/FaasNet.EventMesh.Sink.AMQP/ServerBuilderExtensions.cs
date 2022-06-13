using FaasNet.EventMesh.Sink.AMQP;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder AddAMQPSeed(this ServerBuilder serverBuilder, Action<EventMeshSinkAMQPOptions> amqpOptionsCallback = null)
        {
            serverBuilder.AddAMQPSeed(amqpOptionsCallback);
            return serverBuilder;
        }
    }
}
