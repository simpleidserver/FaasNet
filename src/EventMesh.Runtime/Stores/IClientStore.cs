using EventMesh.Runtime.Models;
using System.Collections.Generic;

namespace EventMesh.Runtime.Stores
{
    public interface IClientStore
    {
        Client Get(string clientId);
        Client GetByActiveSession(string clientId, string sessionId);
        IEnumerable<Client> GetAllBySubscribedTopics(string brokerName, string topicName);
        int Count();
        int CountActiveSessions();
        IEnumerable<Client> GetAll();
        void Add(Client session);
        void Update(Client session);
    }
}
