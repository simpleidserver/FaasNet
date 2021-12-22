using FaasNet.Gateway.Core.Exceptions;
using FaasNet.Gateway.Core.Resources;
using FaasNet.Runtime.Domains.Definitions;
using FaasNet.Runtime.Persistence;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.StateMachines.Queries.Handlers
{
    public class GetLastStateMachineDetailsQueryHandler : IRequestHandler<GetLastStateMachineDetailsQuery, WorkflowDefinitionAggregate>
    {
        private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;

        public GetLastStateMachineDetailsQueryHandler(IWorkflowDefinitionRepository workflowDefinitionRepository)
        {
            _workflowDefinitionRepository = workflowDefinitionRepository;
        }

        public Task<WorkflowDefinitionAggregate> Handle(GetLastStateMachineDetailsQuery request, CancellationToken cancellationToken)
        {
            var workflowDefinition = _workflowDefinitionRepository.Query(true).Where(w => w.Id == request.Id).OrderByDescending(w => w.Version).LastOrDefault();
            if (workflowDefinition == null)
            {
                throw new StateMachineNotFoundException(ErrorCodes.StateMachineExists, string.Format(Global.UnknownStateMachine, request.Id));
            }

            return Task.FromResult(workflowDefinition);
        }
    }
}
