using FaasNet.EventMesh.Sink;
using FaasNet.EventMesh.Sink.AMQP;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAMQPSeed(this IServiceCollection services, Action<EventMeshSinkAMQPOptions> amqpOptionsCallback = null)
        {
            if (amqpOptionsCallback == null)
            {
                services.AddSeed(null);
                services.Configure<EventMeshSinkAMQPOptions>((o) => { });
            }
            else
            {
                var opt = new EventMeshSinkAMQPOptions();
                amqpOptionsCallback(opt);
                services.AddSeed((t) =>
                {
                    t.EventMeshPort = opt.EventMeshPort;
                    t.EventMeshUrl = opt.EventMeshUrl;
                    t.Vpn = opt.Vpn;
                    t.ClientId = opt.ClientId;
                });
                services.Configure(amqpOptionsCallback);
            }

            services.AddTransient<ISinkJob, AMQPSinkJob>();
            return services;
        }
    }
}
