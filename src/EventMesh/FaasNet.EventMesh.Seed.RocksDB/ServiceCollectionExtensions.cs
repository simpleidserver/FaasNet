using FaasNet.EventMesh.Seed.RocksDB;
using FaasNet.EventMesh.Seed.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseSeedRocksDB(this IServiceCollection services, Action<EventMeshSeedRocksDBOptions> callback = null)
        {
            if (callback == null) services.Configure<EventMeshSeedRocksDBOptions>(o => { });
            else services.Configure(callback);
            services.AddSingleton<ISubscriptionStore, SubscriptionStore>();
            return services;
        }
    }
}
