using FaasNet.RaftConsensus.Core.StateMachines;
using System.Collections.Generic;
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
        public EventDefinitionStateMachineStore()
        {

        }

        public void Add(EventDefinitionRecord record)
        {
            throw new System.NotImplementedException();
        }

        public Task BulkUpload(IEnumerable<EventDefinitionRecord> records, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<EventDefinitionRecord> Get(string key, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<EventDefinitionRecord> Get(string key, string vpn, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<(IEnumerable<EventDefinitionRecord>, int)> GetAll(int nbRecords)
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

        public void Update(EventDefinitionRecord record)
        {
            throw new System.NotImplementedException();
        }
    }
}
