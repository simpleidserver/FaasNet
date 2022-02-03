using System.Net;

namespace EventMesh.Runtime.Stores
{
    public interface IClientSessionStore
    {
        Session Get(IPEndPoint edp);
        void Add(Session session);
    }
}
