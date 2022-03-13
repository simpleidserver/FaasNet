using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Persistence;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Gateway.Core.StateMachines.Commands.Handlers
{
    public class AddStateMachineEmptyCommandHandler : IRequestHandler<AddEmptyStateMachineCommand, string>
    {
        private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;

        public AddStateMachineEmptyCommandHandler(IWorkflowDefinitionRepository workflowDefinitionRepository)
        {
            _workflowDefinitionRepository = workflowDefinitionRepository;
        }

        public async Task<string> Handle(AddEmptyStateMachineCommand request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid().ToString();
            var record = WorkflowDefinitionAggregate.CreateEmpty(id, request.Name, request.Description);
            await _workflowDefinitionRepository.Add(record, cancellationToken);
            await _workflowDefinitionRepository.SaveChanges(cancellationToken);
            return id;
        }
    }
}
