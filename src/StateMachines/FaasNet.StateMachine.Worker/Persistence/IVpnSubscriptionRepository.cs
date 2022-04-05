using FaasNet.StateMachine.Worker.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker.Persistence
{
    public interface IVpnSubscriptionRepository
    {
        Task<VpnSubscriptionAggregate> Get(string name, CancellationToken cancellationToken);
        Task<ICollection<VpnSubscriptionAggregate>> GetAll(CancellationToken cancellationToken);
        Task Add(VpnSubscriptionAggregate vpn, CancellationToken cancellation);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
