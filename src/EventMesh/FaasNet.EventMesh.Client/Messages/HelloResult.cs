using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class HelloResult : BaseEventMeshPackage
    {
        public HelloResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.HELLO_RESPONSE;
        public string SessionId { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(SessionId);
        }

        public HelloResult Extract(ReadBufferContext context)
        {
            SessionId = context.NextString();
            return this;
        }
    }
}
