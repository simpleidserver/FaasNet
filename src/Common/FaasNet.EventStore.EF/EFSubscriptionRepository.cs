using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventStore.EF
{
    public class EFSubscriptionRepository : ISubscriptionRepository
    {
        private readonly EventStoreDBContext _dbContext;

        public EFSubscriptionRepository(EventStoreDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Commit(string groupId, string topicName, long offset, CancellationToken cancellationToken)
        {
            var subscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.GroupId == groupId && s.TopicName == topicName);
            if (subscription != null)
            {
                subscription.Offset = offset;
            }
            else
            {
                _dbContext.Subscriptions.Add(new Models.Subscription
                {
                    GroupId = groupId,
                    Offset = offset,
                    TopicName = topicName
                });
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<long?> GetCurrentOffset(string groupId, string topicName, CancellationToken cancellationToken)
        {
            var subscription = await _dbContext.Subscriptions.FirstOrDefaultAsync(s => s.GroupId == groupId && s.TopicName == topicName);
            if (subscription == null)
            {
                return null;
            }

            return subscription.Offset;
        }
    }
}
