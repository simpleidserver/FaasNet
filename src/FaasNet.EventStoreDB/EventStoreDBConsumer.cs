using EventStore.Client;
using FaasNet.Domain;
using FaasNet.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStoreDB
{
    public class EventStoreDBConsumer : IEventStoreConsumer
    {
        public async Task<List<DomainEvent>> Search(string topicName, long offset, CancellationToken cancellationToken)
        {
            var client = new EventStoreClient();
            var result = client.ReadStreamAsync(Direction.Forwards,
                topicName,
                StreamPosition.FromInt64(offset),
                cancellationToken: cancellationToken);
            var evts = await result.ToListAsync(cancellationToken);
            var domainEvts = new List<DomainEvent>();
            foreach (var evt in evts)
            {
                var type = Type.GetType(evt.OriginalEvent.EventType); 
                domainEvts.Add(JsonSerializer.Deserialize(evt.OriginalEvent.Data.ToArray(), type) as DomainEvent);
            }

            return domainEvts;
        }

        public async Task<IDisposable> Subscribe(string topicName, long offset, Func<DomainEvent, Task> callback, CancellationToken cancellationToken)
        {
            var client = new EventStoreClient();
            return await client.SubscribeToStreamAsync(topicName,
                FromStream.After(StreamPosition.FromInt64(offset)),
                async (subscription, evt, cancellationToken) => {
                    var type = Type.GetType(evt.OriginalEvent.EventType);
                    var domainEvt = JsonSerializer.Deserialize(evt.OriginalEvent.Data.ToArray(), type) as DomainEvent;
                    await callback(domainEvt);
                });

        }
    }
}
