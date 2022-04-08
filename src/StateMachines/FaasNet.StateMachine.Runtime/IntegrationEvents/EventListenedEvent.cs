using FaasNet.Domain;

namespace FaasNet.StateMachine.Runtime.IntegrationEvents
{
    public class EventListenedEvent : IntegrationEvent
    {
        public EventListenedEvent(string id, string aggregateId, string stateInstanceId, string vpn, string source, string type, string topic) : base(id, aggregateId) 
        {
            StateInstanceId = stateInstanceId;
            Vpn = vpn;
            Source = source;
            Type = type;
            Topic = topic;
        }

        public string StateInstanceId { get; set; }
        public string Vpn { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public string Topic  { get; set; }
    }
}
