using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.StateMachines.Counter
{
    public class CounterStateMachineStore : IStateMachineRecordStore<CounterRecord>
    {
        private ConcurrentBag<CounterRecord> _records;

        public CounterStateMachineStore()
        {
            _records = new ConcurrentBag<CounterRecord>();
        }

        public void Add(CounterRecord record)
        {
            _records.Add(record);
        }

        public Task BulkUpload(IEnumerable<CounterRecord> records, CancellationToken cancellationToken)
        {
            _records = new ConcurrentBag<CounterRecord>(records);
            return Task.CompletedTask;
        }

        public Task<CounterRecord> Get(string key, CancellationToken cancellationToken)
        {
            return Task.FromResult(_records.FirstOrDefault(r => r.Id == key));
        }

        public IEnumerable<(IEnumerable<CounterRecord>, int)> GetAll(int nbRecords)
        {
            int nbPages = (int)Math.Ceiling((double)_records.Count / nbRecords);
            for(var currentPage = 0; currentPage < nbPages; currentPage++)
                yield return (_records.Skip(currentPage * nbRecords).Take(nbRecords), currentPage);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        public Task Truncate(CancellationToken cancellationToken)
        {
            _records = new ConcurrentBag<CounterRecord>();
            return Task.CompletedTask;
        }

        public void Update(CounterRecord record)
        {
        }
    }
}
