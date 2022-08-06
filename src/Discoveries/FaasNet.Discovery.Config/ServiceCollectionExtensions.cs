using FaasNet.Discovery.Config;
using FaasNet.Peer.Clusters;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigDiscovery(this IServiceCollection services, Action<DiscoveryConfigurationOptions> callback)
        {
            if (callback == null) services.Configure<DiscoveryConfigurationOptions>(o => { });
            else services.Configure(callback);
            var serviceDescriptor = services.FirstOrDefault(s => s.ImplementationType == typeof(IClusterStore));
            if(serviceDescriptor != null) services.Remove(serviceDescriptor);
            services.AddTransient<IClusterStore, ConfigClusterStore>();
            return services;
        }
    }
}
