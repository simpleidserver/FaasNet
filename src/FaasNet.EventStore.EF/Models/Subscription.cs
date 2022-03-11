namespace FaasNet.EventStore.EF.Models
{
    public class Subscription
    {
        public string GroupId { get; set; }
        public string TopicName { get; set; }
        public long Offset { get; set; }
    }
}
