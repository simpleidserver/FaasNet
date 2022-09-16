using FaasNet.RaftConsensus.Core.StateMachines;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.QueueMessage
{
    public interface IQueueMessageStateMachineStore : IStateMachineRecordStore<QueueMessageRecord>
    {

    }

    public class QueueMessageStateMachineStore : IQueueMessageStateMachineStore
    {
        private ConcurrentBag<QueueMessageRecord> _records = new ConcurrentBag<QueueMessageRecord>();

        public void Add(QueueMessageRecord record)
        {
            _records.Add(record);
        }

        public Task BulkUpload(IEnumerable<QueueMessageRecord> records, CancellationToken cancellationToken)
        {
            _records = new ConcurrentBag<QueueMessageRecord>(records);
            return Task.CompletedTask;
        }

        public Task<QueueMessageRecord> Get(string key, CancellationToken cancellationToken)
        {
            return Task.FromResult(_records.FirstOrDefault());
        }

        public IEnumerable<(IEnumerable<QueueMessageRecord>, int)> GetAll(int nbRecords)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task Truncate(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Update(QueueMessageRecord record)
        {
            throw new System.NotImplementedException();
        }
    }
}
