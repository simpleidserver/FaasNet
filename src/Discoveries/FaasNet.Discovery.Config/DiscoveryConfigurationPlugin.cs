using FaasNet.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.Discovery.Config
{
    public class DiscoveryConfigurationPlugin : IPlugin<DiscoveryConfigurationOptions>
    {
        public void Load(IServiceCollection services, DiscoveryConfigurationOptions options)
        {
            services.AddConfigDiscovery(o =>
            {
                o.ClusterNodes = options.ClusterNodes;
            });
        }
    }
}
