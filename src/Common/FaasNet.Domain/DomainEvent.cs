namespace FaasNet.Domain
{
    public abstract class DomainEvent
    {
        public DomainEvent(string id, string aggregateId)
        {
            Id = id;
            AggregateId = aggregateId;
        }

        public string Id { get; private set; }
        public string AggregateId { get; private set; }
        public int AggregateVersion { get; set; }
    }
}
