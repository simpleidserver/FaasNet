using System.Collections.Generic;

namespace FaasNet.EventMesh.StateMachines.Client
{
    public class ClientRemoved
    {
        public string Id { get; set; }
        public string Vpn { get; set; }
        public ICollection<ClientTargetRemoved> Targets { get; set; }
    }

    public class ClientTargetRemoved
    {
        public string Target { get; set; }
        public string EventId { get; set; }
    }
}
