using FaasNet.StateMachine.Runtime.Domains.Subscriptions;
using FaasNet.StateMachine.Runtime.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.EF.Persistence
{
    public class CloudEventSubscriptionRepository : ICloudEventSubscriptionRepository
    {
        private readonly RuntimeDBContext _dbContext;

        public CloudEventSubscriptionRepository(RuntimeDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Add(CloudEventSubscriptionAggregate evt, CancellationToken cancellationToken)
        {
            _dbContext.Subscriptions.Add(evt);
            return Task.CompletedTask;
        }

        public IQueryable<CloudEventSubscriptionAggregate> Query()
        {
            return _dbContext.Subscriptions;
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task Update(IEnumerable<CloudEventSubscriptionAggregate> evts, CancellationToken cancellationToken)
        {
            foreach (var evt in evts)
            {
                _dbContext.Subscriptions.Update(evt);
            }

            return Task.CompletedTask;
        }
    }
}
