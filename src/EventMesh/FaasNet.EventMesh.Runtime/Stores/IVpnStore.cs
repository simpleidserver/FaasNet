using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IVpnStore
    {
        Task<IEnumerable<Vpn>> GetAll(CancellationToken cancellationToken);
        Task<Vpn> Get(string name, CancellationToken cancellationToken);
        Task<Vpn> Get(string clientId, string sessionId, CancellationToken cancellationToken);
        Task<Vpn> Get(string clientId, string urn, string sessionId, CancellationToken cancellationToken);
        void Add(Vpn vpn);
        void Delete(Vpn vpn);
        void Update(Vpn vpn);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
