using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Extensions;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.Persistence.InMemory
{
    public class InMemoryStateMachineDefinitionRepository : IStateMachineDefinitionRepository
    {
        private readonly ConcurrentBag<StateMachineDefinitionAggregate> _defs;

        public InMemoryStateMachineDefinitionRepository(ConcurrentBag<StateMachineDefinitionAggregate> defs)
        {
            _defs = defs;
        }

        public Task Add(StateMachineDefinitionAggregate workflowDef, CancellationToken cancellationToken)
        {
            _defs.Add(workflowDef);
            return Task.CompletedTask;
        }

        public Task Update(StateMachineDefinitionAggregate workflowDef, CancellationToken cancellationToken)
        {
            _defs.Remove(_defs.First(_ => _.Id == workflowDef.Id));
            _defs.Add(workflowDef);
            return Task.CompletedTask;
        }


        public IQueryable<StateMachineDefinitionAggregate> Query()
        {
            return _defs.AsQueryable();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
