using FaasNet.Peer.Clusters;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Discovery.Config
{
    public class ConfigClusterStore : IClusterStore
    {
        private DiscoveryConfigurationOptions _options;

        public ConfigClusterStore(IOptions<DiscoveryConfigurationOptions> options)
        {
            _options = options.Value;
        }

        public Task SelfRegister(ClusterPeer node, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ClusterPeer>> GetAllNodes(CancellationToken cancellationToken)
        {
            IEnumerable<ClusterPeer> result = _options.ClusterNodes;
            return Task.FromResult(result);
        }
    }
}
