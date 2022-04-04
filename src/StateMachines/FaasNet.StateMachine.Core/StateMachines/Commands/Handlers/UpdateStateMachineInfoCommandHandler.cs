using FaasNet.Domain.Exceptions;
using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.Core.Resources;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.StateMachines.Commands.Handlers
{
    public class UpdateStateMachineInfoCommandHandler : IRequestHandler<UpdateStateMachineInfoCommand>
    {
        private readonly IStateMachineDefinitionRepository _stateMachineDefinitionRepository;

        public UpdateStateMachineInfoCommandHandler(IStateMachineDefinitionRepository stateMachineDefinitionRepository)
        {
            _stateMachineDefinitionRepository= stateMachineDefinitionRepository;
        }

        public async Task<Unit> Handle(UpdateStateMachineInfoCommand request, CancellationToken cancellationToken)
        {
            var workflowDefinition = _stateMachineDefinitionRepository.Query().FirstOrDefault(w => w.TechnicalId == request.Id);
            if (workflowDefinition == null)
            {
                throw new NotFoundException(ErrorCodes.UNKNOWN_STATEMACHINE_DEF, string.Format(Global.UnknownStateMachineDef, request.Id));
            }

            workflowDefinition.UpdateInfo(request.Name, request.Description);
            await _stateMachineDefinitionRepository.Update(workflowDefinition, cancellationToken);
            await _stateMachineDefinitionRepository.SaveChanges(cancellationToken);
            return Unit.Value;
        }
    }
}
