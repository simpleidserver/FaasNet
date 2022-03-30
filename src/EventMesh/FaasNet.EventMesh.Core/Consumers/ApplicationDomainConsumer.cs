using FaasNet.EventMesh.Core.ApplicationDomains.Commands;
using FaasNet.StateMachine.IntegrationEvents;
using MassTransit;
using MediatR;
using System;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Core.Consumers
{
    public class ApplicationDomainConsumer : IConsumer<StateMachineDefinitionAddedEvent>
    {
        private readonly IMediator _mediator;

        public ApplicationDomainConsumer(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<StateMachineDefinitionAddedEvent> context)
        {
            await _mediator.Send(new AddApplicationDomainCommand
            {
                Description = "State machine",
                Name = "stateMachine",
                RootTopic = Guid.NewGuid().ToString(),
                Vpn = context.Message.Vpn,
                CorrelationId = context.Message.CorrelationId
            });
        }
    }
}
