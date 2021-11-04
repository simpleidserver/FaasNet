using FaasNet.Runtime.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Runtime.Persistence.InMemory
{
    public class InMemoryCloudEventSubscriptionRepository : ICloudEventSubscriptionRepository
    {
        private readonly ICollection<CloudEventSubscriptionAggregate> _cloudEvents;

        public InMemoryCloudEventSubscriptionRepository(ICollection<CloudEventSubscriptionAggregate> cloudEvents)
        {
            _cloudEvents = cloudEvents;
        }

        public Task Add(CloudEventSubscriptionAggregate evt, CancellationToken cancellationToken)
        {
            _cloudEvents.Add(evt);
            return Task.CompletedTask;
        }

        public IQueryable<CloudEventSubscriptionAggregate> Query()
        {
            return _cloudEvents.AsQueryable();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        public Task Update(IEnumerable<CloudEventSubscriptionAggregate> evt, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
