namespace FaasNet.Domain
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
        public string CorrelationId { get; set; }
    }
}
