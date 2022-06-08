using FaasNet.EventMesh.Sink.RocksDB;
using FaasNet.EventMesh.Sink.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseSinkRocksDB(this IServiceCollection services, Action<EventMeshSinkRocksDBOptions> callback = null)
        {
            if (callback == null) services.Configure<EventMeshSinkRocksDBOptions>(o => { });
            else services.Configure(callback);
            services.AddSingleton<ISubscriptionStore, SubscriptionStore>();
            return services;
        }
    }
}
