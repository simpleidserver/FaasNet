using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.IntegrationEvents;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using MassTransit;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.StateMachines.Commands.Handlers
{
    public class AddStateMachineEmptyCommandHandler : IRequestHandler<AddEmptyStateMachineCommand, string>
    {
        private readonly IStateMachineDefinitionRepository _stateMachineDefinitionRepository;
        private readonly IBusControl _busControl;

        public AddStateMachineEmptyCommandHandler(IStateMachineDefinitionRepository workflowDefinitionRepository, IBusControl busControl)
        {
            _stateMachineDefinitionRepository = workflowDefinitionRepository;
            _busControl = busControl;
        }

        public async Task<string> Handle(AddEmptyStateMachineCommand request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid().ToString();
            var record = StateMachineDefinitionAggregate.CreateEmpty(id, request.Name, request.Description, request.Vpn);
            await _stateMachineDefinitionRepository.Add(record, cancellationToken);
            await _stateMachineDefinitionRepository.SaveChanges(cancellationToken);
            await _busControl.Publish(new StateMachineDefinitionAddedEvent(record.Id, record.Name, record.Description, record.Vpn)
            {
                CorrelationId = record.Id
            }, cancellationToken);
            return id;
        }
    }
}
