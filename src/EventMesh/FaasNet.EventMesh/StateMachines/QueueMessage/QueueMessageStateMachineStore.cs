using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.QueueMessage
{
    public interface IQueueMessageStateMachineStore : IStateMachineRecordStore<QueueMessageRecord>
    {
        Task<QueueMessageRecord> Get(int offset, CancellationToken cancellationToken);
    }

    public class QueueMessageStateMachineStore : IQueueMessageStateMachineStore
    {
        private ConcurrentBag<QueueMessageRecord> _records = new ConcurrentBag<QueueMessageRecord>();

        public void Add(QueueMessageRecord record)
        {
            record.Index = _records.Count + 1;
            _records.Add(record);
        }

        public Task BulkUpload(IEnumerable<QueueMessageRecord> records, CancellationToken cancellationToken)
        {
            _records = new ConcurrentBag<QueueMessageRecord>(records);
            return Task.CompletedTask;
        }

        public Task<QueueMessageRecord> Get(string key, CancellationToken cancellationToken)
        {
            return Task.FromResult(_records.FirstOrDefault(r => r.Id == key));
        }

        public Task<QueueMessageRecord> Get(int offset, CancellationToken cancellationToken)
        {
            return Task.FromResult(_records.SingleOrDefault(r => r.Index == offset));
        }

        public IEnumerable<(IEnumerable<QueueMessageRecord>, int)> GetAll(int nbRecords)
        {
            int nbPages = (int)Math.Ceiling((double)_records.Count / nbRecords);
            for (var currentPage = 0; currentPage < nbPages; currentPage++)
                yield return (_records.Skip(currentPage * nbRecords).Take(nbRecords), currentPage);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        public Task Truncate(CancellationToken cancellationToken)
        {
            _records = new ConcurrentBag<QueueMessageRecord>();
            return Task.CompletedTask;
        }

        public void Update(QueueMessageRecord record)
        {
        }
    }
}
