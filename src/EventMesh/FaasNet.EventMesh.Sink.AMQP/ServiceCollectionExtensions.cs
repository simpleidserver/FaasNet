using FaasNet.EventMesh.Sink;
using FaasNet.EventMesh.Sink.AMQP;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAMQPSeed(this IServiceCollection services, Action<SinkOptions> seedOptionsCallback = null, Action<EventMeshSinkAMQPOptions> amqpOptionsCallback = null)
        {
            services.AddSeed(seedOptionsCallback);
            if (amqpOptionsCallback == null) services.Configure<EventMeshSinkAMQPOptions>((o) => { });
            else services.Configure(amqpOptionsCallback);
            services.AddTransient<ISinkJob, AMQPSinkJob>();
            return services;
        }
    }
}
