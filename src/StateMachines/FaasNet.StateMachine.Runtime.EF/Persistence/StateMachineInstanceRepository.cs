using FaasNet.StateMachine.Runtime.Domains.Instances;
using FaasNet.StateMachine.Runtime.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.EF.Persistence
{
    public class StateMachineInstanceRepository : IStateMachineInstanceRepository
    {
        private readonly RuntimeDBContext _dbContext;

        public StateMachineInstanceRepository(RuntimeDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Add(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            _dbContext.WorkflowInstances.Add(workflowInstance);
            return Task.CompletedTask;
        }

        public IQueryable<StateMachineInstanceAggregate> Query()
        {
            return _dbContext.WorkflowInstances
                .Include(w => w.States).ThenInclude(w => w.Events);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task Update(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken)
        {
            _dbContext.WorkflowInstances.Update(workflowInstance);
            return Task.CompletedTask;
        }
    }
}
