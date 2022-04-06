using FaasNet.Domain;
using FaasNet.EventStore.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore.InMemory
{
    public class InMemoryEventStoreProducer : IEventStoreProducer
    {
        private readonly ConcurrentBag<SerializedEvent> _evts;
        private readonly ConcurrentBag<EventSubscription> _subscriptions;

        public InMemoryEventStoreProducer(ConcurrentBag<SerializedEvent> evts, ConcurrentBag<EventSubscription> subscriptions)
        {
            _evts = evts;
            _subscriptions = subscriptions;
        }

        public async Task<bool> Append<T>(string topicName, T domainEvt, CancellationToken cancellationToken) where T : DomainEvent
        {
            var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(domainEvt));
            _evts.Add(new SerializedEvent
            {
                Payload = payload,
                Topic = topicName,
                Type = domainEvt.GetType().FullName
            });
            var filtredSubscriptions = _subscriptions.Where(s => s.TopicName == topicName);
            foreach(var filteredSubscription in filtredSubscriptions)
            {
                await filteredSubscription.Callback(domainEvt);
            }

            return true;
        }
    }

    public class EventSubscription
    {
        public EventSubscription(string topicName, Func<DomainEvent, Task> callback)
        {
            TopicName = topicName;
            Callback = callback;
        }

        public string TopicName { get; private set; }
        public Func<DomainEvent, Task> Callback { get; private set; }
    }
}
