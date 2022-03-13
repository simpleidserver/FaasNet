using FaasNet.StateMachine.Runtime.Domains.Definitions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.Persistence
{
    public interface IStateMachineDefinitionRepository
    {
        IQueryable<StateMachineDefinitionAggregate> Query();
        Task Add(StateMachineDefinitionAggregate workflowDef, CancellationToken cancellationToken);
        Task Update(StateMachineDefinitionAggregate workflowDef, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}