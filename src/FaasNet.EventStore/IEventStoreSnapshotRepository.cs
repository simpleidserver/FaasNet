using FaasNet.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore
{
    public interface IEventStoreSnapshotRepository
    {
        Task Add<T>(T domain, CancellationToken cancellationToken) where T : AggregateRoot;
        Task<T> GetLatest<T>(string id, CancellationToken cancellationToken) where T : AggregateRoot;
    }
}
