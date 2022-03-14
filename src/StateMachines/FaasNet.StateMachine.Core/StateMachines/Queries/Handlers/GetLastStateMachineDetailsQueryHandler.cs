using FaasNet.Domain.Exceptions;
using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.Core.Resources;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.StateMachines.Queries.Handlers
{
    public class GetLastStateMachineDetailsQueryHandler : IRequestHandler<GetLastStateMachineDetailsQuery, StateMachineDefinitionAggregate>
    {
        private readonly IStateMachineDefinitionRepository _stateMachineDefinitionRepository;

        public GetLastStateMachineDetailsQueryHandler(IStateMachineDefinitionRepository stateMachineDefinitionRepository)
        {
            _stateMachineDefinitionRepository = stateMachineDefinitionRepository;
        }

        public Task<StateMachineDefinitionAggregate> Handle(GetLastStateMachineDetailsQuery request, CancellationToken cancellationToken)
        {
            var workflowDefinition = _stateMachineDefinitionRepository.Query().Where(w => w.Id == request.Id).OrderByDescending(w => w.Version).LastOrDefault();
            if (workflowDefinition == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_STATEMACHINE_DEF, string.Format(Global.UnknownStateMachineDef, request.Id));
            }

            return Task.FromResult(workflowDefinition);
        }
    }
}
