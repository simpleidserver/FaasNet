using FaasNet.Common.Extensions;
using FaasNet.Domain;
using FaasNet.EventStore.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore.InMemory
{
    public class InMemoryEventStoreConsumer : IEventStoreConsumer
    {
        private readonly ConcurrentBag<SerializedEvent> _evts;
        private readonly ConcurrentBag<EventSubscription> _subscriptions;

        public InMemoryEventStoreConsumer(ConcurrentBag<SerializedEvent> evts, ConcurrentBag<EventSubscription> subscriptions)
        {
            _evts = evts;
            _subscriptions = subscriptions;
        }

        public bool IsOffsetSupported => false;

        public Task<List<DomainEvent>> Search(string topicName, long? offset, CancellationToken cancellationToken)
        {
            var filteredEvts = _evts.OrderBy(e => e.CreationDateTime).Where(e => e.Topic == topicName);
            if (offset != null)
            {
                filteredEvts = filteredEvts.Skip((int)offset.Value);
            }

            var result = filteredEvts.Select(e =>
            {
                var type = Type.GetType(e.Type);
                var json = Encoding.UTF8.GetString(e.Payload);
                return JsonSerializer.Deserialize(json, type) as DomainEvent;
            }).ToList();
            return Task.FromResult(result);
        }

        public async Task<IDisposable> Subscribe(string topicName, long? offset, Func<DomainEvent, Task> callback, CancellationToken cancellationToken)
        {
            var filteredEvts = await Search(topicName, offset, cancellationToken);
            foreach(var evt in filteredEvts)
            {
                await callback(evt);
            }

            var subscription = new EventSubscription(topicName, callback);
            _subscriptions.Add(subscription);
            return new SubscriptionResult(_subscriptions, subscription);
        }

        public Task Commit(string groupId, string topicName, long offset, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<long?> GetCurrentOffset(string groupId, string topicName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public class SubscriptionResult : IDisposable
        {
            private readonly ConcurrentBag<EventSubscription> _subscriptions;
            private readonly EventSubscription _subscription;

            public SubscriptionResult(ConcurrentBag<EventSubscription> subscriptions, EventSubscription subscription)
            {
                _subscriptions = subscriptions;
                _subscription = subscription;
            }

            public void Dispose()
            {
                _subscriptions.Remove(_subscription);
            }
        }
    }
}
