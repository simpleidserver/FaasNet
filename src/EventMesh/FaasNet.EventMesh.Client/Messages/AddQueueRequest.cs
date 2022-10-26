using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddQueueRequest : BaseEventMeshPackage
    {
        public AddQueueRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_QUEUE_REQUEST;

        public string Vpn { get; set; }
        public string QueueName { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(QueueName);
        }

        public AddQueueRequest Extract(ReadBufferContext context)
        {
            Vpn = context.NextString();
            QueueName = context.NextString();
            return this;
        }
    }
}
