using FaasNet.RaftConsensus.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public class InMemoryClusterStore : IClusterStore
    {
        private readonly ConcurrentBag<Peer> _peers;

        public InMemoryClusterStore(ConcurrentBag<Peer> peers)
        {
            _peers = peers;
        }

        public Task<IEnumerable<Peer>> GetAllPeers(CancellationToken cancellationToken)
        {
            IEnumerable<Peer> result = _peers.ToArray();
            return Task.FromResult(result);
        }
    }
}
