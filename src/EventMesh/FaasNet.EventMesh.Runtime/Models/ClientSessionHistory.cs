using System;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class ClientSessionHistory
    {
        public ClientSessionState State { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
