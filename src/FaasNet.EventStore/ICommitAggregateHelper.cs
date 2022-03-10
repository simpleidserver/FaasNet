using FaasNet.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore
{
    public interface ICommitAggregateHelper
    {
        Task Commit<T>(T domain, CancellationToken cancellationToken) where T : AggregateRoot;
        Task<T> Get<T>(string aggregateId, CancellationToken cancellationToken) where T : AggregateRoot;
    }
}
