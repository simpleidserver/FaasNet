using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Persistence;
using Microsoft.EntityFrameworkCore;
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

        public Task Update(WorkflowDefinitionAggregate workflowDef, CancellationToken cancellationToken)
        {
            _dbContext.WorkflowDefinitions.Update(workflowDef);
            return Task.CompletedTask;
        }

        public IQueryable<WorkflowDefinitionAggregate> Query()
        {
            return _dbContext.WorkflowDefinitions
                .Include(w => w.States).ThenInclude(s => ((WorkflowDefinitionSwitchState)s).Conditions)
                .Include(w => w.Functions)
                .Include(w => w.Events);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
