
using EventMesh.Runtime;
using EventMesh.Runtime.AMQP;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAMQP(this IServiceCollection services, Action<AMQPOptions> callback = null)
        {
            if (callback != null)
            {
                services.Configure(callback);
            }
            else
            {
                services.Configure<AMQPOptions>(opt => { });
            }

            services.AddSingleton<IMessageConsumer, AMQPConsumer>();
            services.AddSingleton<IMessagePublisher, AMQPPublisher>();
            return services;
        }
    }
}
