using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Seed.Stores
{
    public interface ISubscriptionStore
    {
        Task<int> GetOffset(string jobId, CancellationToken cancellationToken);
        Task IncrementOffset(string jobId, CancellationToken cancellationToken);
    }

    public class InMemorySubscriptionStore : ISubscriptionStore
    {
        private Dictionary<string, int> _subscriptions;

        public InMemorySubscriptionStore()
        {
            _subscriptions = new Dictionary<string, int>();
        }

        public Task<int> GetOffset(string jobId, CancellationToken cancellationToken)
        {
            if (!_subscriptions.ContainsKey(jobId)) return Task.FromResult(0);
            return Task.FromResult(_subscriptions[jobId]);
        }

        public Task IncrementOffset(string jobId, CancellationToken cancellationToken)
        {
            if (!_subscriptions.ContainsKey(jobId)) _subscriptions.Add(jobId, 1);
            else _subscriptions[jobId] = _subscriptions[jobId] + 1;
            return Task.CompletedTask;
        }
    }
}
