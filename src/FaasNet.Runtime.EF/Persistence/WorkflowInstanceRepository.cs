using FaasNet.Runtime.Domains.Instances;
using FaasNet.Runtime.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.EF.Persistence
{
    public class WorkflowInstanceRepository : IWorkflowInstanceRepository
    {
        private readonly RuntimeDBContext _dbContext;

        public WorkflowInstanceRepository(RuntimeDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Add(WorkflowInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            _dbContext.WorkflowInstances.Add(workflowInstance);
            return Task.CompletedTask;
        }

        public IQueryable<WorkflowInstanceAggregate> Query()
        {
            return _dbContext.WorkflowInstances;
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task Update(WorkflowInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            _dbContext.WorkflowInstances.Update(workflowInstance);
            return Task.CompletedTask;
        }
    }
}
