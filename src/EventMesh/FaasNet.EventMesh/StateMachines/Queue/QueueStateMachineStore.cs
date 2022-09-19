using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Queue
{
    public interface IQueueStateMachineStore : IStateMachineRecordStore<QueueRecord>
    {
        Task<IEnumerable<QueueRecord>> Search(string topic, CancellationToken cancellationToken);
    }

    public class QueueStateMachineStore : IQueueStateMachineStore
    {
        private ConcurrentBag<QueueRecord> _records = new ConcurrentBag<QueueRecord>();

        public void Add(QueueRecord record)
        {
            _records.Add(record);
        }

        public Task BulkUpload(IEnumerable<QueueRecord> records, CancellationToken cancellationToken)
        {
            _records = new ConcurrentBag<QueueRecord>(records);
            return Task.CompletedTask;
        }

        public Task<QueueRecord> Get(string key, CancellationToken cancellationToken)
        {
            return Task.FromResult(_records.FirstOrDefault(r => r.QueueName == key));
        }

        public IEnumerable<(IEnumerable<QueueRecord>, int)> GetAll(int nbRecords)
        {
            int nbPages = (int)Math.Ceiling((double)_records.Count / nbRecords);
            for (var currentPage = 0; currentPage < nbPages; currentPage++)
                yield return (_records.Skip(currentPage * nbRecords).Take(nbRecords), currentPage);
        }

        public Task<IEnumerable<QueueRecord>> Search(string topic, CancellationToken cancellationToken)
        {
            var result = _records.Where(r =>
            {
                var regex = new Regex(r.TopicFilter);
                return r.TopicFilter == topic || regex.IsMatch(topic);
            });
            return Task.FromResult(result);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        public Task Truncate(CancellationToken cancellationToken)
        {
            _records = new ConcurrentBag<QueueRecord>();
            return Task.CompletedTask;
        }

        public void Update(QueueRecord record)
        {
        }
    }
}
