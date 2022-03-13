using FaasNet.StateMachine.Runtime.Domains.Subscriptions;
using FaasNet.StateMachine.Runtime.Extensions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Runtime.Persistence.InMemory
{
    public class InMemoryCloudEventSubscriptionRepository : ICloudEventSubscriptionRepository
    {
        private readonly ConcurrentBag<CloudEventSubscriptionAggregate> _cloudEvents;

        public InMemoryCloudEventSubscriptionRepository(ConcurrentBag<CloudEventSubscriptionAggregate> cloudEvents)
        {
            _cloudEvents = cloudEvents;
        }

        public Task Add(CloudEventSubscriptionAggregate evt, CancellationToken cancellationToken)
        {
            _cloudEvents.Add((CloudEventSubscriptionAggregate)evt.Clone());
            return Task.CompletedTask;
        }

        public IQueryable<CloudEventSubscriptionAggregate> Query()
        {
            return _cloudEvents.Select(c => (CloudEventSubscriptionAggregate)c.Clone()).AsQueryable();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }

        public Task Update(IEnumerable<CloudEventSubscriptionAggregate> evts, CancellationToken cancellationToken)
        {
            foreach(var evt in evts)
            {
                Update(evt, cancellationToken);
            }

            return Task.CompletedTask;
        }

        public Task Update(CloudEventSubscriptionAggregate evt, CancellationToken cancellation)
        {
            var record = _cloudEvents.First(c => c.Source == evt.Source && c.Type == evt.Type && c.StateInstanceId == evt.StateInstanceId && c.WorkflowInstanceId == evt.WorkflowInstanceId);
            _cloudEvents.Remove(record);
            _cloudEvents.Add((CloudEventSubscriptionAggregate)evt.Clone());
            return Task.CompletedTask;
        }
    }
}
