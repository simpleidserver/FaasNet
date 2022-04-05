using FaasNet.StateMachine.Worker.Domains;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.StateMachine.Worker.Persistence
{
    public class InMemoryVpnSubscriptionRepository : IVpnSubscriptionRepository
    {
        private readonly ConcurrentBag<VpnSubscriptionAggregate> _vpnSubscriptions;

        public InMemoryVpnSubscriptionRepository(ConcurrentBag<VpnSubscriptionAggregate> vpnSubscriptions)
        {
            _vpnSubscriptions = vpnSubscriptions;
        }

        public Task Add(VpnSubscriptionAggregate vpn, CancellationToken cancellation)
        {
            _vpnSubscriptions.Add(vpn);
            return Task.CompletedTask;
        }

        public Task<VpnSubscriptionAggregate> Get(string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(_vpnSubscriptions.FirstOrDefault(v => v.Vpn == name));
        }

        public Task<ICollection<VpnSubscriptionAggregate>> GetAll(CancellationToken cancellationToken)
        {
            ICollection<VpnSubscriptionAggregate> result = _vpnSubscriptions.ToList();
            return Task.FromResult(result);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
