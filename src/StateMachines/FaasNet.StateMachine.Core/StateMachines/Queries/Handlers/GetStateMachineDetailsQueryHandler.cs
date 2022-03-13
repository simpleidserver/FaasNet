using FaasNet.Domain.Exceptions;
using FaasNet.StateMachine.Core.Resources;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Persistence;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.StateMachines.Queries.Handlers
{
    public class GetStateMachineDetailsQueryHandler : IRequestHandler<GetStateMachineDetailsQuery, StateMachineDefinitionAggregate>
    {
        private readonly IStateMachineDefinitionRepository _workflowDefinitionRepository;

        public GetStateMachineDetailsQueryHandler(IStateMachineDefinitionRepository workflowDefinitionRepository)
        {
            _workflowDefinitionRepository = workflowDefinitionRepository;
        }

        public Task<StateMachineDefinitionAggregate> Handle(GetStateMachineDetailsQuery request, CancellationToken cancellationToken)
        {
            var workflowDefinition = _workflowDefinitionRepository.Query().FirstOrDefault(w => w.TechnicalId == request.Id);
            if (workflowDefinition == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_STATEMACHINE_DEF, string.Format(Global.UnknownStateMachineDef, request.Id));
            }

            return Task.FromResult(workflowDefinition);
        }
    }
}
