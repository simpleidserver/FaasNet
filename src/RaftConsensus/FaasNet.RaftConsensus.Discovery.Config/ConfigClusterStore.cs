using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Discovery.Config
{
    public class ConfigClusterStore : IClusterStore
    {
        private DiscoveryConfigurationOptions _options;

        public ConfigClusterStore(IOptions<DiscoveryConfigurationOptions> options)
        {
            _options = options.Value;
        }

        public Task SelfRegister(ClusterNode node, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ClusterNode>> GetAllNodes(CancellationToken cancellationToken)
        {
            IEnumerable<ClusterNode> result = _options.ClusterNodes;
            return Task.FromResult(result);
        }
    }
}
