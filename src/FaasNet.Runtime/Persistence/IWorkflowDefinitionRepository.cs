using FaasNet.Runtime.Domains.Definitions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Persistence
{
    public interface IWorkflowDefinitionRepository
    {
        IQueryable<WorkflowDefinitionAggregate> Query();
        Task Add(WorkflowDefinitionAggregate workflowDef, CancellationToken cancellationToken);
        Task Update(WorkflowDefinitionAggregate workflowDef, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}