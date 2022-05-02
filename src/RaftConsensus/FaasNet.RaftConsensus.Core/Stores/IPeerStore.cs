﻿using FaasNet.RaftConsensus.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface IPeerStore
    {
        Task<IEnumerable<PeerInfo>> GetAll(CancellationToken cancellationToken);
    }

    public class InMemoryPeerStore : IPeerStore
    {
        private readonly ConcurrentBag<PeerInfo> _peerInfos;

        public InMemoryPeerStore(ConcurrentBag<PeerInfo> peerInfos)
        {
            _peerInfos = peerInfos;
        }

        public Task<IEnumerable<PeerInfo>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<PeerInfo> peerInfos = _peerInfos;
            return Task.FromResult(peerInfos);
        }
    }
}