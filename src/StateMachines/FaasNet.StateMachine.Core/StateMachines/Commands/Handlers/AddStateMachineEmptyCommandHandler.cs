using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Persistence;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.StateMachines.Commands.Handlers
{
    public class AddStateMachineEmptyCommandHandler : IRequestHandler<AddEmptyStateMachineCommand, string>
    {
        private readonly IStateMachineDefinitionRepository _workflowDefinitionRepository;

        public AddStateMachineEmptyCommandHandler(IStateMachineDefinitionRepository workflowDefinitionRepository)
        {
            _workflowDefinitionRepository = workflowDefinitionRepository;
        }

        public async Task<string> Handle(AddEmptyStateMachineCommand request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid().ToString();
            var record = StateMachineDefinitionAggregate.CreateEmpty(id, request.Name, request.Description);
            await _workflowDefinitionRepository.Add(record, cancellationToken);
            await _workflowDefinitionRepository.SaveChanges(cancellationToken);
            return id;
        }
    }
}
