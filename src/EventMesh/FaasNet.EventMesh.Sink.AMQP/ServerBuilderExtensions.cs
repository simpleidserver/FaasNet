using FaasNet.EventMesh.Sink;
using FaasNet.EventMesh.Sink.AMQP;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder AddAMQPSeed(this ServerBuilder serverBuilder, Action<SinkOptions> seedOptionsCallback = null, Action<EventMeshSinkAMQPOptions> amqpOptionsCallback = null)
        {
            serverBuilder.AddAMQPSeed(seedOptionsCallback, amqpOptionsCallback);
            return serverBuilder;
        }
    }
}
