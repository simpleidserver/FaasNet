using FaasNet.RaftConsensus.Core.Stores;
using FaasNet.RaftConsensus.Discovery.Etcd;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEtcdDiscovery(this IServiceCollection services, Action<EtcdOptions> callback = null)
        {
            if (callback != null) services.Configure(callback);
            else services.Configure<EtcdOptions>(o => { });
            var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IClusterStore));
            if (serviceDescriptor != null) services.Remove(serviceDescriptor);
            services.AddTransient<IClusterStore, EtcdClusterStore>();
            return services;
        }
    }
}
