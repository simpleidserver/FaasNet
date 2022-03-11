using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore
{
    public interface ISubscriptionRepository
    {
        Task Commit(string groupId, string topicName, long offset, CancellationToken cancellationToken);
        Task<long?> GetCurrentOffset(string groupId, string topicName, CancellationToken cancellationToken);
    }
}
