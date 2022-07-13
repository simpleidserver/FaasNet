using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Clusters
{
    public interface IClusterStore
    {
        Task SelfRegister(ClusterNode node, CancellationToken cancellationToken);
        Task<IEnumerable<ClusterNode>> GetAllNodes(CancellationToken cancellationToken);
    }

    public class InMemoryClusterStore : IClusterStore
    {
        private readonly ConcurrentBag<ClusterNode> _clusterNodes;

        public InMemoryClusterStore()
        {
            _clusterNodes = new ConcurrentBag<ClusterNode>();
        }

        public InMemoryClusterStore(ConcurrentBag<ClusterNode> clusterNodes)
        {
            _clusterNodes = clusterNodes;
        }

        public Task<IEnumerable<ClusterNode>> GetAllNodes(CancellationToken cancellationToken)
        {
            IEnumerable<ClusterNode> nodes = _clusterNodes;
            return Task.FromResult(nodes);
        }

        public Task SelfRegister(ClusterNode node, CancellationToken cancellationToken)
        {
            _clusterNodes.Add(node);
            return Task.CompletedTask;
        }
    }
}
