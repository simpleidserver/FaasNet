using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Clusters
{
    public interface IClusterStore
    {
        Task SelfRegister(ClusterPeer node, CancellationToken cancellationToken);
        Task<IEnumerable<ClusterPeer>> GetAllNodes(string partitionKey, CancellationToken cancellationToken);
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

        public Task<IEnumerable<ClusterPeer>> GetAllNodes(string partitionKey, CancellationToken cancellationToken)
        {
            IEnumerable<ClusterPeer> nodes = _clusterPeers;
            if (!string.IsNullOrEmpty(partitionKey)) nodes = nodes.Where(n => n.PartitionKey == partitionKey);
            return Task.FromResult(nodes);
        }

        public Task SelfRegister(ClusterPeer node, CancellationToken cancellationToken)
        {
            var clusterPeer = _clusterPeers.FirstOrDefault(p => p.Url == node.Url && p.Port == node.Port);
            if(clusterPeer == null) _clusterPeers.Add(node);
            return Task.CompletedTask;
        }
    }
}
