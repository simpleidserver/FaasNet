using FaasNet.EventMesh.IntegrationEvents;
using FaasNet.StateMachine.Core.Persistence;
using MassTransit;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.StateMachines
{
    public class StateMachineConsumer :
        IConsumer<ApplicationDomainAddedEvent>,
        IConsumer<ApplicationDomainAddFailedEvent>
    {
        private readonly IStateMachineDefinitionRepository _stateMachineDefinitionRepository;

        public StateMachineConsumer(IStateMachineDefinitionRepository stateMachineDefinitionRepository)
        {
            _stateMachineDefinitionRepository = stateMachineDefinitionRepository;
        }

        public async Task Consume(ConsumeContext<ApplicationDomainAddedEvent> context)
        {
            var stateMachine = _stateMachineDefinitionRepository.Query().FirstOrDefault(s => s.Id == context.Message.CorrelationId);
            stateMachine.ApplicationDomainId = context.Message.AggregateId;
            stateMachine.Status = Runtime.Domains.Definitions.StateMachineDefinitionStatus.ACTIVE;
            await _stateMachineDefinitionRepository.Update(stateMachine, CancellationToken.None);
            await _stateMachineDefinitionRepository.SaveChanges(CancellationToken.None);
        }

        public async Task Consume(ConsumeContext<ApplicationDomainAddFailedEvent> context)
        {
            var stateMachine = _stateMachineDefinitionRepository.Query().FirstOrDefault(s => s.Id == context.Message.CorrelationId);
            stateMachine.Status = Runtime.Domains.Definitions.StateMachineDefinitionStatus.INACTIVE;
            await _stateMachineDefinitionRepository.Update(stateMachine, CancellationToken.None);
            await _stateMachineDefinitionRepository.SaveChanges(CancellationToken.None);
        }
    }
}
