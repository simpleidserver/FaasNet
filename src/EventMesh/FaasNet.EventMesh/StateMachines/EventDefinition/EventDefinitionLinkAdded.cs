namespace FaasNet.EventMesh.StateMachines.EventDefinition
{
    public class EventDefinitionLinkAdded
    {
        public string EventId { get; set; }
        public string Vpn { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
    }
}
