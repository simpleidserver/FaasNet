using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface IClusterStore
    {
        Task<IEnumerable<ClusterNode>> GetAllNodes(CancellationToken cancellationToken);
        Task<ClusterNode> GetNode(string url, int port, CancellationToken cancellationToken);
        Task AddNode(ClusterNode node, CancellationToken cancellationToken);
    }

    public class ClusterStore : IClusterStore
    {
        private readonly INodeStateStore _nodeStateStore;
        private readonly ConsensusPeerOptions _options;

        public ClusterStore(INodeStateStore nodeStateStore, IOptions<ConsensusPeerOptions> options)
        {
            _nodeStateStore = nodeStateStore;
            _options = options.Value;
        }

        public async Task<IEnumerable<ClusterNode>> GetAllNodes(CancellationToken cancellationToken)
        {
            var lastEntityTypes = await _nodeStateStore.GetAllLastEntityTypes(StandardEntityTypes.Cluster, cancellationToken);
            var result = lastEntityTypes.Select(et => JsonSerializer.Deserialize<ClusterNode>(et.Value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }));
            return result.Where(r => r.Url != _options.Url || r.Port != _options.Port);
        }

        public async Task<ClusterNode> GetNode(string url, int port, CancellationToken cancellationToken)
        {
            var allNodes = await GetAllNodes(cancellationToken);
            return allNodes.FirstOrDefault(n => n.Port == port && n.Url == url);
        }

        public Task AddNode(ClusterNode node, CancellationToken cancellationToken)
        {
            _nodeStateStore.Add(node.ToNodeState());
            return Task.CompletedTask;
        }
    }
}
