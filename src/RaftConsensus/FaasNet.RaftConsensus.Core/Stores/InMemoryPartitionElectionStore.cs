using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface IPartitionElectionStore
    {
        Task<PartitionElectionRecord> Get(string partitionId, CancellationToken cancellationToken);
        Task<IEnumerable<PartitionElectionRecord>> GetAll(CancellationToken cancellationToken);
        Task Update(PartitionElectionRecord record, CancellationToken cancellationToken);
    }

    public class InMemoryPartitionElectionStore : IPartitionElectionStore
    {
        private readonly ConcurrentBag<PartitionElectionRecord> _partitionElectionRecords;

        public InMemoryPartitionElectionStore()
        {
            _partitionElectionRecords = new ConcurrentBag<PartitionElectionRecord>();
        }

        public InMemoryPartitionElectionStore(ConcurrentBag<PartitionElectionRecord> partitionElectionRecords)
        {
            _partitionElectionRecords = partitionElectionRecords;
        }

        public Task<PartitionElectionRecord> Get(string partitionId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_partitionElectionRecords.SingleOrDefault(p => p.PartitionId == partitionId));
        }

        public Task<IEnumerable<PartitionElectionRecord>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<PartitionElectionRecord> result = _partitionElectionRecords.ToList();
            return Task.FromResult(result);
        }

        public Task Update(PartitionElectionRecord record, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class PartitionElectionRecord
    {
        private PartitionElectionRecord() { }

        public PeerStates PeerState { get; set; }
        public DateTime? ExpirationElectionDateTime { get; set; }
        public int Quorum { get; set; }
        public int NbPositiveVote { get; set; }
        public string PartitionId { get; set; }
        public long ConfirmedTermIndex { get; set; }
        public long TermIndex { get; set; }

        public static PartitionElectionRecord Build(string partitionId)
        {
            return new PartitionElectionRecord { PartitionId = partitionId };
        }

        public void Upgrade()
        {
            ConfirmedTermIndex++;
            TermIndex = ConfirmedTermIndex;
        }

        public void Reset()
        {
            TermIndex = ConfirmedTermIndex;
        }

        public void Increment()
        {
            TermIndex = ConfirmedTermIndex + 1;
        }
    }
}
