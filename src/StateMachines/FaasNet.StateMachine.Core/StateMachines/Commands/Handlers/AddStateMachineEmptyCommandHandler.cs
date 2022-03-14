using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.StateMachines.Commands.Handlers
{
    public class AddStateMachineEmptyCommandHandler : IRequestHandler<AddEmptyStateMachineCommand, string>
    {
        private readonly IStateMachineDefinitionRepository _stateMachineDefinitionRepository;

        public AddStateMachineEmptyCommandHandler(IStateMachineDefinitionRepository workflowDefinitionRepository)
        {
            _stateMachineDefinitionRepository = workflowDefinitionRepository;
        }

        public async Task<string> Handle(AddEmptyStateMachineCommand request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid().ToString();
            var record = StateMachineDefinitionAggregate.CreateEmpty(id, request.Name, request.Description);
            await _stateMachineDefinitionRepository.Add(record, cancellationToken);
            await _stateMachineDefinitionRepository.SaveChanges(cancellationToken);
            return id;
        }
    }
}
