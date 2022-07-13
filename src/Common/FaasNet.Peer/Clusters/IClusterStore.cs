using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Clusters
{
    public interface IClusterStore
    {
        Task SelfRegister(ClusterPeer node, CancellationToken cancellationToken);
        Task<IEnumerable<ClusterPeer>> GetAllNodes(CancellationToken cancellationToken);
    }

    public class InMemoryClusterStore : IClusterStore
    {
        private readonly ConcurrentBag<ClusterPeer> _clusterPeers;

        public InMemoryClusterStore()
        {
            _clusterPeers = new ConcurrentBag<ClusterPeer>();
        }

        public InMemoryClusterStore(ConcurrentBag<ClusterPeer> clusterNodes)
        {
            _clusterPeers = clusterNodes;
        }

        public Task<IEnumerable<ClusterPeer>> GetAllNodes(CancellationToken cancellationToken)
        {
            IEnumerable<ClusterPeer> nodes = _clusterPeers;
            return Task.FromResult(nodes);
        }

        public Task SelfRegister(ClusterPeer node, CancellationToken cancellationToken)
        {
            _clusterPeers.Add(node);
            return Task.CompletedTask;
        }
    }
}
