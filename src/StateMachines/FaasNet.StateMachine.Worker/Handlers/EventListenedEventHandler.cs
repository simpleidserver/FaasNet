using FaasNet.StateMachine.Runtime;
using FaasNet.StateMachine.Runtime.IntegrationEvents;
using FaasNet.StateMachine.Worker.Domains;
using FaasNet.StateMachine.Worker.Persistence;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker.Handlers
{
    public class EventListenedEventHandler : IIntegrationEventHandler<EventListenedEvent>
    {
        private readonly ICloudEventSubscriptionRepository _cloudEventSubscriptionRepository;
        private readonly ILogger<EventListenedEventHandler> _logger;

        public EventListenedEventHandler(ICloudEventSubscriptionRepository cloudEventSubscriptionRepository, ILogger<EventListenedEventHandler> logger)
        {
            _cloudEventSubscriptionRepository = cloudEventSubscriptionRepository;
            _logger = logger;
        }

        public async Task Process(EventListenedEvent evt, CancellationToken cancellationToken)
        {
            _logger.LogInformation("State machine instance {stateMachineInstanceId} is listening the event, RootTopic = {rootTopic}, MessageTopic = {messageTopic}, Source = {source}, Type = {type}, Vpn = {vpn}", evt.AggregateId, evt.RootTopic, evt.Topic, evt.Source, evt.Type, evt.Vpn);
            StateMachineRuntimeMeter.IncrementActiveSubscriptions();
            await _cloudEventSubscriptionRepository.Add(CloudEventSubscriptionAggregate.Create(evt.AggregateId, evt.StateInstanceId, evt.RootTopic, evt.Source, evt.Type, evt.Vpn, evt.Topic), CancellationToken.None);
            await _cloudEventSubscriptionRepository.SaveChanges(cancellationToken);
        }
    }
}
