using FaasNet.Runtime.Domains.IntegrationEvents;
using FaasNet.Runtime.Domains.Subscriptions;
using FaasNet.Runtime.Persistence;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Infrastructure.Handlers
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
            await _cloudEventSubscriptionRepository.Add(CloudEventSubscriptionAggregate.Create(evt.AggregateId, evt.StateInstanceId, evt.Source, evt.Type), CancellationToken.None);
            await _cloudEventSubscriptionRepository.SaveChanges(cancellationToken);
        }
    }
}
