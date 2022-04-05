using FaasNet.StateMachine.Runtime.IntegrationEvents;
using FaasNet.StateMachine.Worker.Domains;
using FaasNet.StateMachine.Worker.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker.Handlers
{
    public class EventListenedEventHandler : IIntegrationEventHandler<EventListenedEvent>
    {
        private readonly ICloudEventSubscriptionRepository _cloudEventSubscriptionRepository;

        public EventListenedEventHandler(ICloudEventSubscriptionRepository cloudEventSubscriptionRepository)
        {
            _cloudEventSubscriptionRepository = cloudEventSubscriptionRepository;
        }

        public async Task Process(EventListenedEvent evt, CancellationToken cancellationToken)
        {
            await _cloudEventSubscriptionRepository.Add(CloudEventSubscriptionAggregate.Create(evt.AggregateId, evt.StateInstanceId, evt.Source, evt.Type, evt.Vpn), CancellationToken.None);
            await _cloudEventSubscriptionRepository.SaveChanges(cancellationToken);
        }
    }
}
