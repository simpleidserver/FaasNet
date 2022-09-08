using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class ReadMessageRequest : BaseEventMeshPackage
    {
        public ReadMessageRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.READ_MESSAGE_REQUEST;
        public int Offset { get; set; }
        public string SessionId { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger(Offset);
            context.WriteString(SessionId);
        }

        public ReadMessageRequest Extract(ReadBufferContext context)
        {
            Offset = context.NextInt();
            SessionId = context.NextString();
            return this;
        }
    }
}
