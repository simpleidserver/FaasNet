namespace FaasNet.EventStore.EF.Models
{
    public class Snapshot
    {
        public int Version { get; set; }
        public string AggregateId { get; set; }
        public int LastEvtOffset { get; set; }
        public string Type { get; set; }
        public string SerializedContent { get; set; }
    }
}
