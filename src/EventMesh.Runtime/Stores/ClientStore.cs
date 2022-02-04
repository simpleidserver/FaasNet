using EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace EventMesh.Runtime.Stores
{
    public class ClientStore : IClientStore
    {
        private readonly ICollection<Client> _clients;

        public ClientStore()
        {
            _clients = new List<Client>();
        }

        public Client Get(string clientId)
        {
            return _clients.FirstOrDefault(c => c.ClientId == clientId);
        }

        public Client GetByActiveSession(IPEndPoint edp)
        {
            return _clients.FirstOrDefault(s => s.HasActiveSession(edp));
        }

        public void Add(Client session)
        {
            _clients.Add(session);
        }

        public void Update(Client client)
        {
            var existingClient = _clients.FirstOrDefault(c => c.ClientId == client.ClientId);
            if (existingClient != null)
            {
                _clients.Remove(existingClient);
            }

            _clients.Add(client);
        }
    }
}
