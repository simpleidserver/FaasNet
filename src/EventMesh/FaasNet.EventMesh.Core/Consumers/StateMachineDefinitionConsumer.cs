using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Core.ApplicationDomains.Commands;
using FaasNet.EventMesh.Core.ApplicationDomains.Queries.Results;
using FaasNet.EventMesh.Core.Clients.Commands;
using FaasNet.EventMesh.IntegrationEvents;
using FaasNet.StateMachine.IntegrationEvents;
using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace FaasNet.EventMesh.Core.Consumers
{
    public class StateMachineDefinitionConsumer : IConsumer<StateMachineDefinitionAddedEvent>
    {
        private readonly IMediator _mediator;
        private readonly IBusControl _busControl;

        public StateMachineDefinitionConsumer(IMediator mediator, IBusControl busControl)
        {
            _mediator = mediator;
            _busControl = busControl;
        }

        public async Task Consume(ConsumeContext<StateMachineDefinitionAddedEvent> context)
        {
            try
            {
                string applicationDomainId = null;
                using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var name = context.Message.Name.Replace(" ", string.Empty);
                    var clientId = Guid.NewGuid().ToString();
                    await _mediator.Send(new AddClientCommand
                    {
                        ClientId = clientId,
                        Purposes = new List<int>
                        {
                            UserAgentPurpose.PUB.Code,
                            UserAgentPurpose.SUB.Code
                        },
                        Vpn = context.Message.Vpn
                    });
                    var applicationDomain = await _mediator.Send(new AddApplicationDomainCommand
                    {
                        Name = name,
                        Description = context.Message.Description,
                        RootTopic = Guid.NewGuid().ToString(),
                        Vpn = context.Message.Vpn
                    });
                    await _mediator.Send(new UpdateApplicationDomainCommand
                    {
                        ApplicationDomainId = applicationDomain.Id,
                        Applications = new List<ApplicationResult>
                        {
                            new ApplicationResult
                            {
                                Id = Guid.NewGuid().ToString(),
                                ClientId = clientId,
                                Title = "StateMachineClient",
                                Description = "StateMachine",
                                PosX = 500,
                                PosY = 100
                            }
                        }
                    });
                    applicationDomainId = applicationDomain.Id;
                    transactionScope.Complete();
                }

                await _busControl.Publish(new ApplicationDomainAddedEvent(applicationDomainId)
                {
                    CorrelationId = context.Message.CorrelationId
                });
            }
            catch
            {
                await _busControl.Publish(new ApplicationDomainAddFailedEvent(null)
                {
                    CorrelationId = context.Message.CorrelationId
                });
            }
        }
    }
}
