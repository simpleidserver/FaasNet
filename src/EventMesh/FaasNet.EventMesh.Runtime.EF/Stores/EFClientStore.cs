using FaasNet.EventMesh.Runtime.Models;
using FaasNet.EventMesh.Runtime.Stores;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.EF.Stores
{
    public class EFClientStore : IClientStore
    {
        public EFClientStore()
        {

        }

        public void Add(Client client)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Client>> GetAllByVpn(string name, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Client> GetByBridgeSession(string clientId, string bridgeUrn, string bridgeSessionId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Client> GetByClientId(string vpn, string clientId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Client> GetById(string id, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<Client> GetBySession(string clientId, string clientSessionId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(Client client)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
