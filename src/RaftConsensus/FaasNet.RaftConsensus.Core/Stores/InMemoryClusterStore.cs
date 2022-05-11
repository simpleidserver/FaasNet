using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.Models;
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

    public class InMemoryClusterStore : IClusterStore
    {
        private readonly INodeStateStore _nodeStateStore;

        public InMemoryClusterStore(INodeStateStore nodeStateStore)
        {
            _nodeStateStore = nodeStateStore;
        }

        public async Task<IEnumerable<ClusterNode>> GetAllNodes(CancellationToken cancellationToken)
        {
            var lastEntityTypes = await _nodeStateStore.GetAllLastEntityTypes(StandardEntityTypes.Cluster, cancellationToken);
            lastEntityTypes = lastEntityTypes.OrderBy(e => e.EntityVersion);
            var result = lastEntityTypes.Select(et => JsonSerializer.Deserialize<ClusterNode>(et.Value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }));
            return result;
        }

        public async Task<ClusterNode> GetNode(string url, int port, CancellationToken cancellationToken)
        {
            var allNodes = await GetAllNodes(cancellationToken);
            return allNodes.FirstOrDefault(n => n.Port == port && n.Url == url);
        }

        public async Task AddNode(ClusterNode node, CancellationToken cancellationToken)
        {
            var lastEntityType = await _nodeStateStore.GetLastEntityType(StandardEntityTypes.Cluster, cancellationToken);
            var nodeState = node.ToNodeState();
            if (lastEntityType != null) nodeState.EntityVersion = lastEntityType.EntityVersion + 1;
            _nodeStateStore.Add(nodeState);
        }
    }
}
