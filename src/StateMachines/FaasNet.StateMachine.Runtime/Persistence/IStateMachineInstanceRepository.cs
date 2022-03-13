using FaasNet.StateMachine.Runtime.Domains;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Persistence
{
    public interface IStateMachineInstanceRepository
    {
        IQueryable<StateMachineInstanceAggregate> Query();
        Task Add(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken);
        Task Update(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
