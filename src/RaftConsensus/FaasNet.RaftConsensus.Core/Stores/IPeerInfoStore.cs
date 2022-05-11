using FaasNet.RaftConsensus.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface IPeerInfoStore
    {
        Task<IEnumerable<PeerInfo>> GetAll(CancellationToken cancellationToken);
        void Update(PeerInfo peerInfo);
        void Add(PeerInfo peerInfo);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }

    public class InMemoryPeerInfoStore : IPeerInfoStore
    {
        private readonly ConcurrentBag<PeerInfo> _peerInfos;

        public InMemoryPeerInfoStore(ConcurrentBag<PeerInfo> peerInfos)
        {
            _peerInfos = peerInfos;
        }

        public Task<IEnumerable<PeerInfo>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<PeerInfo> peerInfos = _peerInfos;
            return Task.FromResult(peerInfos);
        }

        public void Update(PeerInfo peerInfo)
        {
        }

        public void Add(PeerInfo peerInfo)
        {
            _peerInfos.Add(peerInfo);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
