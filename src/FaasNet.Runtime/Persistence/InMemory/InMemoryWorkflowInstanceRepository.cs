using FaasNet.Runtime.Domains;
using FaasNet.Runtime.Extensions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Persistence.InMemory
{
    public class InMemoryWorkflowInstanceRepository : IWorkflowInstanceRepository
    {
        private readonly ConcurrentBag<WorkflowInstanceAggregate> _instances;

        public InMemoryWorkflowInstanceRepository(ConcurrentBag<WorkflowInstanceAggregate> instances)
        {
            _instances = instances;
        }

        public Task Add(WorkflowInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            _instances.Add((WorkflowInstanceAggregate)workflowInstance.Clone());
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

        public Task Update(WorkflowInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            _instances.Remove(_instances.First(i => i.Id == workflowInstance.Id));
            _instances.Add((WorkflowInstanceAggregate)workflowInstance.Clone());
            return Task.CompletedTask;
        }
    }
}
