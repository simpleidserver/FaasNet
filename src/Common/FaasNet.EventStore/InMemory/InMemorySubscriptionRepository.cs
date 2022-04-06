using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore.InMemory
{
    public class InMemorySubscriptionRepository : ISubscriptionRepository
    {
        private readonly ConcurrentBag<Subscription> _subscriptions;

        public InMemorySubscriptionRepository()
        {
            _subscriptions = new ConcurrentBag<Subscription>();
        }

        public Task Commit(string groupId, string topicName, long offset, CancellationToken cancellationToken)
        {
            var subscription = _subscriptions.FirstOrDefault(s => s.GroupId == groupId && s.TopicName == topicName);
            if (subscription == null)
            {
                subscription.Offset = offset;
            }
            else
            {
                _subscriptions.Add(new Subscription
                {
                    GroupId = groupId,
                    TopicName = topicName,
                    Offset = offset
                });
            }

            return Task.CompletedTask;
        }

        public Task<long?> GetCurrentOffset(string groupId, string topicName, CancellationToken cancellationToken)
        {
            var subscription = _subscriptions.FirstOrDefault(s => s.GroupId == groupId && s.TopicName == topicName);
            if (subscription == null)
            {
                return Task.FromResult((long?)null);
            }

            return Task.FromResult((long?)subscription.Offset);
        }

        public class Subscription
        {
            public string GroupId { get; set; }
            public string TopicName { get; set; }
            public long Offset { get; set; }
        }
    }
}
