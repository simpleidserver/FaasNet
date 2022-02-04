using EventMesh.Runtime.Models;
using System.Net;

namespace EventMesh.Runtime.Stores
{
    public interface IClientStore
    {
        Client Get(string clientId);
        Client GetByActiveSession(IPEndPoint edp);
        void Add(Client session);
        void Update(Client session);
    }
}
