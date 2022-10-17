namespace FaasNet.EventMesh.StateMachines.ApplicationDomain
{
    public class ApplicationDomainLinkRemoved
    {
        public string Vpn { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string EventId { get; set; }
    }
}
