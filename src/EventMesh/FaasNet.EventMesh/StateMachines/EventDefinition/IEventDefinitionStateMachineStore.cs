using FaasNet.EventMesh.StateMachines.Queue;
using FaasNet.RaftConsensus.Core.StateMachines;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.EventDefinition
{
    public interface IEventDefinitionStateMachineStore : IStateMachineRecordStore<EventDefinitionRecord>
    {
        Task<EventDefinitionRecord> Get(string key, string vpn, CancellationToken cancellationToken);
    }

    public class EventDefinitionStateMachineStore : IEventDefinitionStateMachineStore
    {
        private ConcurrentBag<EventDefinitionRecord> _records = new ConcurrentBag<EventDefinitionRecord>();

        public EventDefinitionStateMachineStore()
        {

        }

        public void Add(EventDefinitionRecord record)
        {
            _records.Add(record);
        }

        public Task BulkUpload(IEnumerable<EventDefinitionRecord> records, CancellationToken cancellationToken)
        {
            _records = new ConcurrentBag<EventDefinitionRecord>(records);
            return Task.CompletedTask;
        }

        public Task<EventDefinitionRecord> Get(string key, CancellationToken cancellationToken)
        {
            return Task.FromResult(_records.FirstOrDefault(r => r.Id == key));
        }

        public Task<EventDefinitionRecord> Get(string key, string vpn, CancellationToken cancellationToken)
        {
            return Task.FromResult(_records.FirstOrDefault(r => r.Id == key && r.Vpn == vpn));
        }

        public IEnumerable<(IEnumerable<EventDefinitionRecord>, int)> GetAll(int nbRecords)
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
            _records = new ConcurrentBag<EventDefinitionRecord>();
            return Task.CompletedTask;
        }

        public void Update(EventDefinitionRecord record)
        {
        }
    }
}
