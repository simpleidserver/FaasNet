using FaasNet.Domain;
using FaasNet.StateMachine.Runtime.Domains.Instances;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachineInstance.Persistence
{
    public interface IStateMachineInstanceRepository
    {
        Task<StateMachineInstanceAggregate> Get(string id, CancellationToken cancellationToken);
        Task<BaseSearchResult<StateMachineInstanceAggregate>> Search(SearchWorkflowInstanceParameter parameter, CancellationToken cancellationToken);
        Task Add(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken);
        Task Update(StateMachineInstanceAggregate workflowInstance, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
