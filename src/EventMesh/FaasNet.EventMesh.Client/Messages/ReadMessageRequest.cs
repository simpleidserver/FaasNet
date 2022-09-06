using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class ReadMessageRequest : BaseEventMeshPackage
    {
        public ReadMessageRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.READ_MESSAGE_REQUEST;
        public long Offset { get; set; }
        public string QueueName { get; set; }
        public string SessionId { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteLong(Offset);
            context.WriteString(QueueName);
            context.WriteString(SessionId);
        }

        public ReadMessageRequest Extract(ReadBufferContext context)
        {
            Offset = context.NextLong();
            QueueName = context.NextString();
            SessionId = context.NextString();
            return this;
        }
    }
}
