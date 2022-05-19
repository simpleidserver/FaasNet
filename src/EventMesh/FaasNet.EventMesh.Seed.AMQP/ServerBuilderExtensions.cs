using FaasNet.EventMesh.Seed;
using FaasNet.EventMesh.Seed.AMQP;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder AddAMQPSeed(this ServerBuilder serverBuilder, Action<SeedOptions> seedOptionsCallback = null, Action<EventMeshSeedAMQPOptions> amqpOptionsCallback = null)
        {
            serverBuilder.AddAMQPSeed(seedOptionsCallback, amqpOptionsCallback);
            return serverBuilder;
        }
    }
}
