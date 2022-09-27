using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Extensions;
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
        Task<QueueRecord> Get(string key, string vpn, CancellationToken cancellationToken);
        Task<IEnumerable<QueueRecord>> Search(string vpn, string topic, CancellationToken cancellationToken);
        Task<GenericSearchResult<QueueRecord>> Find(FilterQuery filter, CancellationToken cancellationToken);
        Task<IEnumerable<string>> Find(string name, CancellationToken cancellationToken);
    }

    public class QueueStateMachineStore : IQueueStateMachineStore
    {
        private ConcurrentBag<QueueRecord> _records = new ConcurrentBag<QueueRecord>();

        public QueueStateMachineStore()
        {
        }

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

        public Task<QueueRecord> Get(string key, string vpn, CancellationToken cancellationToken)
        {
            return Task.FromResult(_records.FirstOrDefault(r => r.QueueName == key && r.Vpn == vpn));
        }

        public IEnumerable<(IEnumerable<QueueRecord>, int)> GetAll(int nbRecords)
        {
            int nbPages = (int)Math.Ceiling((double)_records.Count / nbRecords);
            for (var currentPage = 0; currentPage < nbPages; currentPage++)
                yield return (_records.Skip(currentPage * nbRecords).Take(nbRecords), currentPage);
        }

        public Task<IEnumerable<QueueRecord>> Search(string vpn, string topic, CancellationToken cancellationToken)
        {
            var result = _records.Where(r =>
            {
                var regex = new Regex(r.TopicFilter);
                return (r.TopicFilter == topic || regex.IsMatch(topic)) && r.Vpn == vpn;
            });
            return Task.FromResult(result);
        }

        public Task<GenericSearchResult<QueueRecord>> Find(FilterQuery filter, CancellationToken cancellationToken)
        {
            IQueryable<QueueRecord> filtered = _records.AsQueryable();
            if(filter.Comparison != null) filtered = filtered.Filter(filter.Comparison);
            IEnumerable<QueueRecord> res = filtered.InvokeOrderBy(filter.SortBy, filter.SortOrder).ToList();
            var totalRecords = res.Count();
            res = res.Skip(filter.NbRecords * filter.Page).Take(filter.NbRecords);
            var nbPages = (int)Math.Ceiling((decimal)totalRecords / filter.NbRecords);
            return Task.FromResult(new GenericSearchResult<QueueRecord>
            {
                Records = res,
                TotalPages = nbPages,
                TotalRecords = totalRecords
            });
        }

        public Task<IEnumerable<string>> Find(string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(_records.OrderBy(r => r.QueueName).Where(r => string.IsNullOrWhiteSpace(name) || r.QueueName.StartsWith(name, StringComparison.InvariantCultureIgnoreCase)).Take(10).Select(r => r.QueueName));
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
