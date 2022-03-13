using FaasNet.StateMachine.Runtime.Domains.IntegrationEvents;
using FaasNet.StateMachine.Runtime.Domains.Subscriptions;
using FaasNet.StateMachine.Runtime.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Infrastructure.Handlers
{
    public class EventUnlistenedEventHandler : IIntegrationEventHandler<EventUnlistenedEvent>
    {
        private readonly ICloudEventSubscriptionRepository _cloudEventSubscriptionRepository;

        public EventUnlistenedEventHandler(ICloudEventSubscriptionRepository cloudEventSubscriptionRepository)
        {
            _cloudEventSubscriptionRepository = cloudEventSubscriptionRepository;
        }

        public async Task Process(EventUnlistenedEvent evt, CancellationToken cancellationToken)
        {
            var sub = _cloudEventSubscriptionRepository.Query().FirstOrDefault(c => c.WorkflowInstanceId == evt.AggregateId && c.StateInstanceId == evt.StateInstanceId && c.Source == evt.Source && c.Type == evt.Type);
            if (sub == null)
            {
                return;
            }

            sub.IsConsumed = true;
            await _cloudEventSubscriptionRepository.Update(new List<CloudEventSubscriptionAggregate> { sub }, cancellationToken);
            await _cloudEventSubscriptionRepository.SaveChanges(cancellationToken);
        }
    }
}
