using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Persistence;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.EF.Persistence
{
    public class StateMachineDefinitionRepository : IStateMachineDefinitionRepository
    {
        private readonly RuntimeDBContext _dbContext;

        public StateMachineDefinitionRepository(RuntimeDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Add(StateMachineDefinitionAggregate workflowDef, CancellationToken cancellationToken)
        {
            _dbContext.WorkflowDefinitions.Add(workflowDef);
            return Task.CompletedTask;
        }

        public Task Update(StateMachineDefinitionAggregate workflowDef, CancellationToken cancellationToken)
        {
            _dbContext.WorkflowDefinitions.Update(workflowDef);
            return Task.CompletedTask;
        }

        public IQueryable<StateMachineDefinitionAggregate> Query()
        {
            return _dbContext.WorkflowDefinitions;
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
