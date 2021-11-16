using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.EF.Persistence
{
    public class WorkflowDefinitionRepository : IWorkflowDefinitionRepository
    {
        private readonly RuntimeDBContext _dbContext;

        public WorkflowDefinitionRepository(RuntimeDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Add(WorkflowDefinitionAggregate workflowDef, CancellationToken cancellationToken)
        {
            _dbContext.WorkflowDefinitions.Add(workflowDef);
            return Task.CompletedTask;
        }

        public IQueryable<WorkflowDefinitionAggregate> Query()
        {
            return _dbContext.WorkflowDefinitions;
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
