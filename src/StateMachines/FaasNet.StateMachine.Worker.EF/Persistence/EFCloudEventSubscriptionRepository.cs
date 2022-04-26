using FaasNet.StateMachine.Worker.Domains;
using FaasNet.StateMachine.Worker.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker.EF.Persistence
{
    public class EFCloudEventSubscriptionRepository : ICloudEventSubscriptionRepository
    {
        private readonly WorkerDBContext _dbContext;

        public EFCloudEventSubscriptionRepository(WorkerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> NbActiveSubscriptions(CancellationToken cancellationToken)
        {
            return _dbContext.Subscriptions.Where(c => !c.IsConsumed).CountAsync(cancellationToken);
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

        public Task Update(IEnumerable<CloudEventSubscriptionAggregate> evt, CancellationToken cancellationToken)
        {
            _dbContext.Subscriptions.UpdateRange(evt);
            return Task.CompletedTask;
        }
    }
}
