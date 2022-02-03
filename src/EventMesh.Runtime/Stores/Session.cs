using EventMesh.Runtime.Messages;
using System.Net;

namespace EventMesh.Runtime.Stores
{
    public class Session
    {
        public Session(IPEndPoint endpoint, UserAgent userAgent)
        {
            Endpoint = endpoint;
            UserAgent = userAgent;
        }

        public IPEndPoint Endpoint { get; set; }
        public UserAgent UserAgent { get; set; }
    }
}
