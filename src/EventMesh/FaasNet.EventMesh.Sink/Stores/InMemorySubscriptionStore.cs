using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Sink.Stores
{
    public interface ISubscriptionStore
    {
        Task<long> GetOffset(string jobId, string topic, CancellationToken cancellationToken);
        Task IncrementOffset(string jobId, string topic, CancellationToken cancellationToken);
    }

    public class InMemorySubscriptionStore : ISubscriptionStore
    {
        private ICollection<SubscriptionResult> _subscriptionLst;

        public InMemorySubscriptionStore()
        {
            _subscriptionLst = new List<SubscriptionResult>();
        }

        public Task<long> GetOffset(string jobId, string topic, CancellationToken cancellationToken)
        {
            var subscription = _subscriptionLst.FirstOrDefault(s => s.JobId == jobId && s.Topic == topic);
            if (subscription == null) return Task.FromResult((long)0);
            return Task.FromResult(subscription.Offset);
        }

        public Task IncrementOffset(string jobId,  string topic, CancellationToken cancellationToken)
        {
            var subscription = _subscriptionLst.FirstOrDefault(s => s.JobId == jobId && s.Topic == topic);
            if (subscription == null)
            {
                _subscriptionLst.Add(new SubscriptionResult { JobId = jobId, Topic = topic, Offset = 1 });
                return Task.CompletedTask;
            }

            subscription.Offset = subscription.Offset + 1;
            return Task.CompletedTask;
        }

        private class SubscriptionResult
        {
            public string JobId { get; set; }
            public string Topic { get; set; }
            public long Offset { get; set; }
        }
    }
}
