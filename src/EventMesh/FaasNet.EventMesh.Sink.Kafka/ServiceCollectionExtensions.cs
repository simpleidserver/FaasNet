using FaasNet.EventMesh.Sink;
using FaasNet.EventMesh.Sink.Kafka;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaSeed(this IServiceCollection services, Action<KafkaSinkOptions> kafkaOptionsCallback = null)
        {
            if (kafkaOptionsCallback == null)
            {
                services.AddSeed(null);
                services.Configure<KafkaSinkOptions>((o) => { });
            }
            else
            {
                var opt = new KafkaSinkOptions();
                kafkaOptionsCallback(opt);
                services.AddSeed((t) =>
                {
                    t.EventMeshPort = opt.EventMeshPort;
                    t.EventMeshUrl = opt.EventMeshUrl;
                    t.Vpn = opt.Vpn;
                    t.ClientId = opt.ClientId;
                });
                services.Configure(kafkaOptionsCallback);
            }

            services.AddTransient<ISinkJob, KafkaSinkJob>();
            return services;
        }
    }
}
