using EventStore.Client;
using FaasNet.Domain;
using FaasNet.EventStore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStoreDB
{
    public class EventStoreDBConsumer : IEventStoreConsumer
    {
        private readonly EventStoreDBOptions _options;

        public EventStoreDBConsumer(IOptions<EventStoreDBOptions> options)
        {
            _options = options.Value;
        }

        public bool IsOffsetSupported => false;

        public async Task<List<DomainEvent>> Search(string topicName, long? offset, CancellationToken cancellationToken)
        {
            var domainEvts = new List<DomainEvent>();
            try
            {
                var settings = EventStoreClientSettings.Create(_options.ConnectionString);
                var client = new EventStoreClient(settings);
                var position = offset == null ? StreamPosition.Start : StreamPosition.FromInt64(offset.Value);
                var result = client.ReadStreamAsync(Direction.Forwards,
                    topicName,
                    position,
                    cancellationToken: cancellationToken);
                var evts = await result.ToListAsync(cancellationToken);
                foreach (var evt in evts)
                {
                    var type = Type.GetType(evt.OriginalEvent.EventType);
                    domainEvts.Add(JsonSerializer.Deserialize(evt.OriginalEvent.Data.ToArray(), type) as DomainEvent);
                }
            }
            catch { }

            return domainEvts;
        }

        public async Task<IDisposable> Subscribe(string topicName, long? offset, Func<DomainEvent, Task> callback, CancellationToken cancellationToken)
        {
            var settings = EventStoreClientSettings.Create(_options.ConnectionString);
            var client = new EventStoreClient(settings);
            var fromStream = offset == null ? FromStream.Start : FromStream.After(StreamPosition.FromInt64(offset.Value));
            return await client.SubscribeToStreamAsync(topicName,
                fromStream,
                async (subscription, evt, cancellationToken) => {
                    var type = Type.GetType(evt.OriginalEvent.EventType);
                    var json = Encoding.UTF8.GetString(evt.OriginalEvent.Data.ToArray());
                    var domainEvt = JsonSerializer.Deserialize(json, type) as DomainEvent;
                    await callback(domainEvt);
                });

        }

        public Task Commit(string groupId, string topicName, long offset, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<long?> GetCurrentOffset(string groupId, string topicName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}
