using EventMesh.Runtime;
using EventMesh.Runtime.Kafka;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafka(this IServiceCollection services, Action<KafkaOptions> callback = null)
        {
            if (callback != null)
            {
                services.Configure(callback);
            }
            else
            {
                services.Configure<KafkaOptions>(opt => { });
            }

           services.AddSingleton<IMessageConsumer, KafkaConsumer>();
           services.AddSingleton<IMessagePublisher, KafkaPublisher>();
            return services;
        }
    }
}
