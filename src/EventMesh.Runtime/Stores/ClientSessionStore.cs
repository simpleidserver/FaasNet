using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace EventMesh.Runtime.Stores
{
    public class ClientSessionStore : IClientSessionStore
    {
        private readonly ICollection<Session> _sessions;

        public ClientSessionStore()
        {
            _sessions = new List<Session>();
        }

        public Session Get(IPEndPoint edp)
        {
            return _sessions.FirstOrDefault(s => s.Endpoint.Equals(edp));
        }

        public void Add(Session session)
        {
            _sessions.Add(session);
        }
    }
}
