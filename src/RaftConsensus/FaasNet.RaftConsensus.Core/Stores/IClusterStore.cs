using FaasNet.RaftConsensus.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface IClusterStore
    {
        Task<IEnumerable<ClusterNode>> GetAllPeers(CancellationToken cancellationToken);
    }

    public class InMemoryClusterStore : IClusterStore
    {
        private readonly ConcurrentBag<ClusterNode> _peers;

        public InMemoryClusterStore(ConcurrentBag<ClusterNode> peers)
        {
            _peers = peers;
        }

        public Task<IEnumerable<ClusterNode>> GetAllPeers(CancellationToken cancellationToken)
        {
            IEnumerable<ClusterNode> result = _peers.ToArray();
            return Task.FromResult(result);
        }
    }
}
