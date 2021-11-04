using FaasNet.Runtime.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Persistence.InMemory
{
    public class InMemoryWorkflowInstanceRepository : IWorkflowInstanceRepository
    {
        private readonly ICollection<WorkflowInstanceAggregate> _instances;

        public InMemoryWorkflowInstanceRepository(ICollection<WorkflowInstanceAggregate> instances)
        {
            _instances = instances;
        }

        public Task Add(WorkflowInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            _instances.Add(workflowInstance);
            return Task.CompletedTask;
        }

        public IQueryable<WorkflowInstanceAggregate> Query()
        {
            return _instances.AsQueryable();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        public Task Update(WorkflowDefinitionAggregate workflowInstance, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
