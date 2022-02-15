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

        public Client GetByActiveSession(string clientId, string sessionId)
        {
            return _clients.FirstOrDefault(s => s.ClientId == clientId && s.HasActiveSession(sessionId));
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

        public int Count()
        {
            return _clients.Count();
        }

        public int CountActiveSessions()
        {
            return _clients.SelectMany(c => c.Sessions).Count(c => c.State == ClientSessionState.ACTIVE);
        }

        public IEnumerable<Client> GetAll()
        {
            return _clients;
        }

        public IEnumerable<Client> GetAllBySubscribedTopics(string brokerName, string topicName)
        {
            return _clients.Where(c => c.ActiveSessions.Any(a => a.HasTopic(topicName, brokerName)));
        }
    }
}
