using FaasNet.EventMesh.Seed;
using FaasNet.EventMesh.Seed.AMQP;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAMQPSeed(this IServiceCollection services, Action<SeedOptions> seedOptionsCallback = null, Action<EventMeshSeedAMQPOptions> amqpOptionsCallback = null)
        {
            services.AddSeed(seedOptionsCallback);
            if (amqpOptionsCallback == null) services.Configure<EventMeshSeedAMQPOptions>((o) => { });
            else services.Configure(amqpOptionsCallback);
            services.AddTransient<ISeedJob, AMQPSeedJob>();
            return services;
        }
    }
}
