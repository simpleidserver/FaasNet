using FaasNet.EventMesh.Protocols;
using FaasNet.EventMesh.Protocols.AMQP;
using FaasNet.EventMesh.Protocols.AMQP.Handlers;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAMQPProtocol(this IServiceCollection services, Action<EventMeshAMQPOptions> callback)
        {
            if (callback == null) services.Configure<EventMeshAMQPOptions>((o) => { });
            services.Configure(callback);
            services.AddLogging();
            services.AddTransient<IProxy, AMQPProxy>();
            services.AddTransient<IRequestHandler, OpenHandler>();
            services.AddTransient<IRequestHandler, SASLInitHandler>();
            services.AddTransient<IRequestHandler, BeginHandler>();
            services.AddTransient<IRequestHandler, AttachHandler>();
            services.AddTransient<IRequestHandler, TransferHandler>();
            return services;
        }
    }
}
