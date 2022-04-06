using FaasNet.Domain;
using FaasNet.EventStore.EF.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore.EF
{
    public class EFEventStoreSnapshotRepository : IEventStoreSnapshotRepository
    {
        private readonly EventStoreDBContext _dbContext;

        public EFEventStoreSnapshotRepository(EventStoreDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add<T>(T domain, CancellationToken cancellationToken) where T : AggregateRoot
        {
            var snapshot = new Snapshot
            {
                AggregateId = domain.Id,
                Type = typeof(T).AssemblyQualifiedName,
                LastEvtOffset = domain.LastEvtOffset,
                Version = domain.Version,
                SerializedContent = JsonSerializer.Serialize(domain)
            };
            _dbContext.Snapshots.Add(snapshot);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<T> GetLatest<T>(string id, CancellationToken cancellationToken) where T : AggregateRoot
        {
            var result = await _dbContext.Snapshots.OrderByDescending(s => s.Version).FirstOrDefaultAsync(s => s.AggregateId == id, cancellationToken);
            if (result == null)
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(result.SerializedContent);
        }
    }
}
