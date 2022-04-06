using FaasNet.Domain;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore.InMemory
{
    public class InMemoryEventStoreSnapshotRepository : IEventStoreSnapshotRepository
    {
        private readonly ConcurrentBag<Snapshot> _snapshots;

        public InMemoryEventStoreSnapshotRepository()
        {
            _snapshots = new ConcurrentBag<Snapshot>();
        }

        public Task Add<T>(T domain, CancellationToken cancellationToken) where T : AggregateRoot
        {
            var snapshot = new Snapshot
            {
                AggregateId = domain.Id,
                Type = typeof(T).AssemblyQualifiedName,
                LastEvtOffset = domain.LastEvtOffset,
                Version = domain.Version,
                SerializedContent = JsonSerializer.Serialize(domain)
            };
            _snapshots.Add(snapshot);
            return Task.CompletedTask;
        }

        public Task<T> GetLatest<T>(string id, CancellationToken cancellationToken) where T : AggregateRoot
        {
            var result = _snapshots.OrderByDescending(s => s.Version).First(s => s.AggregateId == id);
            if (result == null)
            {
                return Task.FromResult((T)null);
            }

            return Task.FromResult(JsonSerializer.Deserialize<T>(result.SerializedContent));
        }

        public class Snapshot
        {
            public int Version { get; set; }
            public string AggregateId { get; set; }
            public int LastEvtOffset { get; set; }
            public string Type { get; set; }
            public string SerializedContent { get; set; }
        }
    }
}
