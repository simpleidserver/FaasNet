using FaasNet.RaftConsensus.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface IClusterStore
    {
        Task<IEnumerable<ClusterNode>> GetAllNodes(CancellationToken cancellationToken);
    }

    public class InMemoryClusterStore : IClusterStore
    {
        private readonly ConcurrentBag<ClusterNode> _nodes;

        public InMemoryClusterStore(ConcurrentBag<ClusterNode> nodes)
        {
            _nodes = nodes;
        }

        public Task<IEnumerable<ClusterNode>> GetAllNodes(CancellationToken cancellationToken)
        {
            IEnumerable<ClusterNode> result = _nodes.ToArray();
            return Task.FromResult(result);
        }
    }
}
