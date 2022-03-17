using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public class VpnStore : IVpnStore
    {
        private readonly ICollection<Vpn> _vpns;

        public VpnStore(ICollection<Vpn> vpns)
        {
            _vpns = vpns;
        }

        public void Add(Vpn vpn)
        {
            _vpns.Add(vpn);
        }

        public Task<Vpn> Get(string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(_vpns.FirstOrDefault(v => v.Name == name));
        }

        public Task<Vpn> Get(string clientId, string sessionId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_vpns.FirstOrDefault(v => v.Clients.Any(c => c.ClientId == clientId && c.Sessions.Any(s => s.Id == sessionId))));
        }

        public Task<Vpn> Get(string clientId, string urn, string sessionId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_vpns.FirstOrDefault(v => v.Clients.Any(c => c.ClientId == clientId && c.Sessions.Any(s => s.Bridges.Any(b => b.Urn == urn && b.SessionId == sessionId)))));
        }

        public void Update(Vpn vpn)
        {
            _vpns.Remove(_vpns.First(v => v.Name == vpn.Name));
            _vpns.Add(vpn);
        }

        public void Delete(Vpn vpn)
        {
            _vpns.Remove(_vpns.First(v => v.Name == vpn.Name));
        }

        public Task<IEnumerable<Vpn>> GetAll(CancellationToken cancellationToken)
        {
            IEnumerable<Vpn> result = _vpns;
            return Task.FromResult(result);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
