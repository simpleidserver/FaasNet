using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public class ClientStore : IClientStore
    {
        private readonly ICollection<Client> _clients;

        public ClientStore(ICollection<Client> clients)
        {
            _clients = clients;
        }

        public void Add(Client client)
        {
            _clients.Add(client);
        }

        public Task<IEnumerable<Client>> GetAllByVpn(string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(_clients.Where(c => c.Vpn == name));
        }

        public Task<Client> GetByBridgeSession(string clientId, string bridgeUrn, string bridgeSessionId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_clients.FirstOrDefault(c => c.ClientId == clientId && c.Sessions.Any(s => s.Bridges.Any(b => b.Urn == bridgeUrn && b.SessionId == bridgeSessionId))));
        }

        public Task<Client> GetByClientId(string vpn, string clientId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_clients.FirstOrDefault(c => c.Vpn == vpn && c.ClientId == clientId));
        }

        public Task<Client> GetById(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_clients.FirstOrDefault(c => c.Id == id));
        }

        public Task<Client> GetBySession(string clientId, string clientSessionId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_clients.FirstOrDefault(c => c.ClientId == clientId && c.Sessions.Any(s => s.Id == clientSessionId)));
        }

        public void Remove(Client client)
        {
            _clients.Remove(client);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
