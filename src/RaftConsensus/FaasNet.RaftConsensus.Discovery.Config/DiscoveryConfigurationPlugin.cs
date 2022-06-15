using FaasNet.EventMesh.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.RaftConsensus.Discovery.Config
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
