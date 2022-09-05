﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaasNet.Partition
{
    public interface IPartitionPeerStore
    {
        Task<IEnumerable<DirectPartitionPeer>> GetAll();
        Task<DirectPartitionPeer> Get(string partitionId);
        Task Add(DirectPartitionPeer peer);
    }

    public class InMemoryPartitionPeerStore : IPartitionPeerStore
    {
        private readonly ConcurrentBag<DirectPartitionPeer> _partitions;

        public InMemoryPartitionPeerStore(ConcurrentBag<DirectPartitionPeer> partitions)
        {
            _partitions = partitions;
        }

        public Task<IEnumerable<DirectPartitionPeer>> GetAll()
        {
            IEnumerable<DirectPartitionPeer> result = _partitions;
            return Task.FromResult(result);
        }

        public Task<DirectPartitionPeer> Get(string partitionId)
        {
            return Task.FromResult(_partitions.SingleOrDefault(p => p.PartitionKey == partitionId));
        }

        public Task Add(DirectPartitionPeer peer)
        {
            _partitions.Add(peer);
            return Task.CompletedTask;
        }
    }
}