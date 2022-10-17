namespace FaasNet.EventMesh.StateMachines.ApplicationDomain
{
    public class ApplicationDomainLinkAdded
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public string EventId { get; set; }
        public string Vpn { get; set; }
    }
}
