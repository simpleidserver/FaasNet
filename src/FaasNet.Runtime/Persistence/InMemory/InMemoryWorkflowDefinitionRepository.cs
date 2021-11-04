using FaasNet.Runtime.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Persistence.InMemory
{
    public class InMemoryWorkflowDefinitionRepository : IWorkflowDefinitionRepository
    {
        private readonly ICollection<WorkflowDefinitionAggregate> _defs;

        public InMemoryWorkflowDefinitionRepository(ICollection<WorkflowDefinitionAggregate> defs)
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
