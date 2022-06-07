using FaasNet.EventMesh.Sink;
using FaasNet.EventMesh.Sink.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSeed(this IServiceCollection services, Action<SinkOptions> seedOptionsCallback)
        {
            if (seedOptionsCallback == null) services.Configure<SinkOptions>(o => { });
            else services.Configure(seedOptionsCallback);
            services.AddSingleton<ISubscriptionStore, InMemorySubscriptionStore>();
            return services;
        }
    }
}
