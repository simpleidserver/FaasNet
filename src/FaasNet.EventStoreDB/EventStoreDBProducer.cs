using EventStore.Client;
using FaasNet.Domain;
using FaasNet.EventStore;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStoreDB
{
    public class EventStoreDBProducer : IEventStoreProducer
    {
        public EventStoreDBProducer()
        {

        }

        public async Task<bool> Append<T>(string topicName, T domainEvt, CancellationToken cancellationToken) where T : DomainEvent
        {
            var client = new EventStoreClient();
            var payload = JsonSerializer.SerializeToUtf8Bytes(domainEvt);
            var evtData = new EventData(Uuid.NewUuid(), typeof(T).AssemblyQualifiedName, payload);
            await client.AppendToStreamAsync(topicName,
                StreamState.Any,
                new[] { evtData },
                cancellationToken: cancellationToken);
            return true;
        }
    }
}
