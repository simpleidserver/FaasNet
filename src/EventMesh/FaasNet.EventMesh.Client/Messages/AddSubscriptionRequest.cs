using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddSubscriptionRequest : BaseEventMeshPackage
    {
        public AddSubscriptionRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_SUBSCRIPTION_REQUEST;
        public string QueueName { get; set; }
        public string Vpn { get; set; }
        public string Topic { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(QueueName);
            context.WriteString(Vpn);
            context.WriteString(Topic);
        }

        public AddSubscriptionRequest Extract(ReadBufferContext context)
        {
            QueueName = context.NextString();
            Vpn = context.NextString();
            Topic = context.NextString();
            return this;
        }
    }
}
