using EventStore.Client;
using FaasNet.Domain;
using FaasNet.EventStore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStoreDB
{
    public class EventStoreDBProducer : IEventStoreProducer
    {
        private readonly EventStoreDBOptions _options;

        public EventStoreDBProducer(IOptions<EventStoreDBOptions> options)
        {
            _options = options.Value;
        }


        public async Task<bool> Append<T>(string topicName, T domainEvt, CancellationToken cancellationToken) where T : DomainEvent
        {
            var settings = EventStoreClientSettings.Create(_options.ConnectionString);
            var client = new EventStoreClient(settings);
            var type = domainEvt.GetType();
            var payload = JsonSerializer.SerializeToUtf8Bytes(domainEvt, type);
            var evtData = new EventData(Uuid.NewUuid(), type.AssemblyQualifiedName, payload);
            await client.AppendToStreamAsync(topicName,
                StreamState.Any,
                new[] { evtData },
                cancellationToken: cancellationToken);
            return true;
        }
    }
}
