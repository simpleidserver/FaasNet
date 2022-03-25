using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IClientStore
    {
        Task<Client> GetById(string id, CancellationToken cancellationToken);
        Task<Client> GetByClientId(string vpn, string clientId, CancellationToken cancellationToken);
        Task<Client> GetBySession(string clientId, string clientSessionId, CancellationToken cancellationToken);
        Task<Client> GetByBridgeSession(string clientId, string bridgeUrn, string bridgeSessionId, CancellationToken cancellationToken);
        Task<IEnumerable<Client>> GetAllByVpn(string name, CancellationToken cancellationToken);
        void Add(Client client);
        void Remove(Client client);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
