using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IClientStore
    {
        Task<Models.Client> GetById(string id, CancellationToken cancellationToken);
        Task<Models.Client> GetByClientId(string vpn, string clientId, CancellationToken cancellationToken);
        Task<Models.Client> GetBySession(string clientId, string clientSessionId, CancellationToken cancellationToken);
        Task<Models.Client> GetByBridgeSession(string clientId, string bridgeUrn, string bridgeSessionId, CancellationToken cancellationToken);
        Task<IEnumerable<Models.Client>> GetAllByVpn(string name, CancellationToken cancellationToken);
        void Add(Models.Client client);
        void Remove(Models.Client client);
        Task<int> SaveChanges(CancellationToken cancellationToken);
    }
}
