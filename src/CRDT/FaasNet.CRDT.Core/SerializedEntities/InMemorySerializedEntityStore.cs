using FaasNet.Common.Extensions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.CRDT.Core.SerializedEntities
{
    public interface ISerializedEntityStore
    {
        Task<SerializedEntity> Get(string id, CancellationToken cancellationToken);
        Task Update(SerializedEntity serializedEntity, CancellationToken cancellationToken);
        Task<IEnumerable<SerializedEntity>> GetAll(CancellationToken cancellationToken);
    }

    public class InMemorySerializedEntityStore : ISerializedEntityStore
    {
        private readonly ConcurrentBag<SerializedEntity> _entities;

        public InMemorySerializedEntityStore()
        {
            _entities = new ConcurrentBag<SerializedEntity>();
        }

        public InMemorySerializedEntityStore(ConcurrentBag<SerializedEntity> entities)
        {
            _entities = entities;
        }

        public Task<SerializedEntity> Get(string id, CancellationToken cancellationToken)
        {
            var result = _entities.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(result);
        }

        public Task<IEnumerable<SerializedEntity>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<SerializedEntity> result = _entities.ToArray();
            return Task.FromResult(result);
        }

        public Task Update(SerializedEntity serializedEntity, CancellationToken cancellationToken)
        {
            var result = _entities.FirstOrDefault(e => e.Id == serializedEntity.Id);
            if (result == null) _entities.Remove(result);
            _entities.Add(result);
            return Task.CompletedTask;
        }
    }
}
