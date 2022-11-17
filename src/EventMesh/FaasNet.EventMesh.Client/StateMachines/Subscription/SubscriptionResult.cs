using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Subscription
{
    public class SubscriptionResult : ISerializable
    {
        public string Id { get; set; }
        public string QueueName { get; set; }
        public string EventId { get; set; }
        public string Vpn { get; set; }
        public string Topic { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            QueueName = context.NextString();
            EventId = context.NextString();
            Vpn = context.NextString();
            Topic = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(QueueName);
            context.WriteString(EventId);
            context.WriteString(Vpn);
            context.WriteString(Topic);
        }
    }
}
