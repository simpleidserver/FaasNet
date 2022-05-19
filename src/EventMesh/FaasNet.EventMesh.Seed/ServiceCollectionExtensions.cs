using FaasNet.EventMesh.Seed;
using FaasNet.EventMesh.Seed.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSeed(this IServiceCollection services, Action<SeedOptions> seedOptionsCallback)
        {
            if (seedOptionsCallback == null) services.Configure<SeedOptions>(o => { });
            else services.Configure(seedOptionsCallback);
            services.AddSingleton<ISubscriptionStore, InMemorySubscriptionStore>();
            return services;
        }
    }
}
