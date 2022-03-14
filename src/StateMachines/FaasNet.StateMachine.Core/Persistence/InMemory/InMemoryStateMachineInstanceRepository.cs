using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Extensions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.Persistence.InMemory
{
    public class InMemoryStateMachineInstanceRepository : IStateMachineInstanceRepository
    {
        private readonly ConcurrentBag<StateMachineInstanceAggregate> _instances;

        public InMemoryStateMachineInstanceRepository(ConcurrentBag<StateMachineInstanceAggregate> instances)
        {
            _instances = instances;
        }

        public Task Add(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            _instances.Add((StateMachineInstanceAggregate)workflowInstance.Clone());
            return Task.CompletedTask;
        }

        public IQueryable<StateMachineInstanceAggregate> Query()
        {
            return _instances.Select(i => (StateMachineInstanceAggregate)i.Clone()).AsQueryable();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        public Task Update(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            var currentInstance = _instances.First(i => i.Id == workflowInstance.Id);
            _instances.Remove(_instances.First(i => i.Id == workflowInstance.Id));
            var newRecord = (StateMachineInstanceAggregate)workflowInstance.Clone();
            _instances.Add(newRecord);
            return Task.CompletedTask;
        }
    }
}
