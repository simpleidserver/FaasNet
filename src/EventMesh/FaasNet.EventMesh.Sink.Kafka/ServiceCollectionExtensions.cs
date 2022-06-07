using FaasNet.EventMesh.Sink;
using FaasNet.EventMesh.Sink.Kafka;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaSeed(this IServiceCollection services, Action<SinkOptions> seedOptionsCallback = null, Action<KafkaSinkOptions> kafkaOptionsCallback = null)
        {
            services.AddSeed(seedOptionsCallback);
            if (kafkaOptionsCallback == null) services.Configure<KafkaSinkOptions>((o) => { });
            else services.Configure(kafkaOptionsCallback);
            services.AddTransient<ISinkJob, KafkaSinkJob>();
            return services;
        }
    }
}
