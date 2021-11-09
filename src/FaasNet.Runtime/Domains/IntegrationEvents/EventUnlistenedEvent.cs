namespace FaasNet.Runtime.Domains.IntegrationEvents
{
    public class EventUnlistenedEvent : IntegrationEvent
    {
        public EventUnlistenedEvent(string id, string aggregateId, string stateInstanceId, string source, string type) : base(id, aggregateId)
        {
            StateInstanceId = stateInstanceId;
            Source = source;
            Type = type;
        }

        public string StateInstanceId { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
    }
}
