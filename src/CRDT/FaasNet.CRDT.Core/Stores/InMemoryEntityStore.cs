using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.CRDT.Core.Stores
{
    public interface IEntityStore
    {
        Task<SerializedEntity> Get(string id, CancellationToken cancellationToken);
    }

    public class InMemoryEntityStore : IEntityStore
    {
        private readonly ConcurrentBag<SerializedEntity> _entities;

        public InMemoryEntityStore()
        {
            _entities = new ConcurrentBag<SerializedEntity>();
        }

        public Task<SerializedEntity> Get(string id, CancellationToken cancellationToken)
        {
            var result = _entities.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(result);
        }
    }
}
