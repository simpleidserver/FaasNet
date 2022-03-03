using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IClientStore
    {
        Client Get(string clientId);
        Client GetByActiveSession(string clientId, string sessionId);
        int Count();
        int CountActiveSessions();
        IEnumerable<Client> GetAll();
        void Add(Client session);
        void Update(Client session);
    }
}
