namespace FaasNet.StateMachine.Runtime.Domains.IntegrationEvents
{
    public class IntegrationEvent
    {
        public IntegrationEvent(string id, string aggregateId) 
        {
            Id = id;
            AggregateId = aggregateId;
        }

        public string Id { get; set; }
        public string AggregateId { get; set; }
    }
}
