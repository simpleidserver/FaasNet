using FaasNet.Runtime.Domains.Definitions;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Persistence.InMemory
{
    public class InMemoryWorkflowDefinitionRepository : IWorkflowDefinitionRepository
    {
        private readonly ConcurrentBag<WorkflowDefinitionAggregate> _defs;

        public InMemoryWorkflowDefinitionRepository(ConcurrentBag<WorkflowDefinitionAggregate> defs)
        {
            _defs = defs;
        }

        public Task Add(WorkflowDefinitionAggregate workflowDef, CancellationToken cancellationToken)
        {
            _defs.Add(workflowDef);
            return Task.CompletedTask;
        }

        public IQueryable<WorkflowDefinitionAggregate> Query()
        {
            return _defs.AsQueryable();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
