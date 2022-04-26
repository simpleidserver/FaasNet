using FaasNet.StateMachine.Worker.Domains;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker.Persistence
{
    public interface ICloudEventSubscriptionRepository
    {
        Task<int> NbActiveSubscriptions(CancellationToken cancellationToken);
        IQueryable<CloudEventSubscriptionAggregate> Query();
        Task Add(CloudEventSubscriptionAggregate evt, CancellationToken cancellationToken);
        Task Update(IEnumerable<CloudEventSubscriptionAggregate> evt, CancellationToken cancellationToken);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
