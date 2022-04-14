using FaasNet.EventMesh.IntegrationEvents;
using FaasNet.StateMachine.Core.Persistence;
using FaasNet.StateMachine.Runtime.Domains.Definitions;
using FaasNet.StateMachine.Runtime.Domains.Enums;
using MassTransit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Core.StateMachines
{
    public class StateMachineConsumer :
        IConsumer<ApplicationDomainAddedEvent>,
        IConsumer<ApplicationDomainAddFailedEvent>,
        IConsumer<ApplicationDomainStateMachineEvtUpdatedEvent>
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
            stateMachine.RootTopic = context.Message.RootTopic;
            stateMachine.Status = StateMachineDefinitionStatus.ACTIVE;
            await _stateMachineDefinitionRepository.Update(stateMachine, CancellationToken.None);
            await _stateMachineDefinitionRepository.SaveChanges(CancellationToken.None);
        }

        public async Task Consume(ConsumeContext<ApplicationDomainAddFailedEvent> context)
        {
            var stateMachine = _stateMachineDefinitionRepository.Query().FirstOrDefault(s => s.Id == context.Message.CorrelationId);
            stateMachine.Status = StateMachineDefinitionStatus.INACTIVE;
            await _stateMachineDefinitionRepository.Update(stateMachine, CancellationToken.None);
            await _stateMachineDefinitionRepository.SaveChanges(CancellationToken.None);
        }

        public async Task Consume(ConsumeContext<ApplicationDomainStateMachineEvtUpdatedEvent> context)
        {
            var stateMachine = _stateMachineDefinitionRepository.Query().OrderByDescending(s => s.Version).FirstOrDefault(s => s.ApplicationDomainId == context.Message.AggregateId);
            if (stateMachine == null)
            {
                return;
            }

            stateMachine.Events = context.Message.Evts.Select(evt => new StateMachineDefinitionEvent
            {
                Kind = evt.IsConsumed ? StateMachineDefinitionEventKinds.Consumed : StateMachineDefinitionEventKinds.Produced,
                Source = $"urn:{evt.MessageId}",
                Metadata = new Newtonsoft.Json.Linq.JObject
                {
                    { "topic", evt.TopicName }
                }
            }).ToList();
            stateMachine.UpdateDateTime = DateTime.UtcNow;
            await _stateMachineDefinitionRepository.Update(stateMachine, CancellationToken.None);
            await _stateMachineDefinitionRepository.SaveChanges(CancellationToken.None);
        }
    }
}
