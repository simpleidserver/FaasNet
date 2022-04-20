using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public class ClientStore : IClientStore
    {
        private readonly ICollection<Models.Client> _clients;

        public ClientStore(ICollection<Models.Client> clients)
        {
            _clients = clients;
        }

        public void Add(Models.Client client)
        {
            _clients.Add(client);
        }

        public Task<IEnumerable<Models.Client>> GetAllByVpn(string name, CancellationToken cancellationToken)
        {
            return Task.FromResult(_clients.Where(c => c.Vpn == name));
        }

        public Task<Models.Client> GetByBridgeSession(string clientId, string bridgeUrn, string bridgeSessionId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_clients.FirstOrDefault(c => c.ClientId == clientId && c.Sessions.Any(s => s.Bridges.Any(b => b.Urn == bridgeUrn && b.SessionId == bridgeSessionId))));
        }

        public Task<Models.Client> GetByClientId(string vpn, string clientId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_clients.FirstOrDefault(c => c.Vpn == vpn && c.ClientId == clientId));
        }

        public Task<Models.Client> GetById(string id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_clients.FirstOrDefault(c => c.Id == id));
        }

        public Task<Models.Client> GetBySession(string clientId, string clientSessionId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_clients.FirstOrDefault(c => c.ClientId == clientId && c.Sessions.Any(s => s.Id == clientSessionId)));
        }

        public Task CloseAllActiveSessions(CancellationToken cancellationToken)
        {
            foreach(var client in _clients)
            {
                client.CloseActiveSession();
            }

            return Task.CompletedTask;
        }

        public void Remove(Models.Client client)
        {
            _clients.Remove(client);
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
