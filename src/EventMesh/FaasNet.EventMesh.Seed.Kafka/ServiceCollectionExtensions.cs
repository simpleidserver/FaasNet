using FaasNet.EventMesh.Seed;
using FaasNet.EventMesh.Seed.Kafka;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaSeed(this IServiceCollection services, Action<SeedOptions> seedOptionsCallback = null, Action<KafkaSeedOptions> kafkaOptionsCallback = null)
        {
            services.AddSeed(seedOptionsCallback);
            if (kafkaOptionsCallback == null) services.Configure<KafkaSeedOptions>((o) => { });
            else services.Configure(kafkaOptionsCallback);
            services.AddTransient<ISeedJob, KafkaSeedJob>();
            return services;
        }
    }
}
