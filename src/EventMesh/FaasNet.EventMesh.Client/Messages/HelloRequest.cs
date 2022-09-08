using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class HelloRequest : BaseEventMeshPackage
    {
        public HelloRequest(string seq) : base(seq)
        {
        }


        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string QueueName { get; set; }
        public ClientPurposeTypes Purpose { get; set; }

        public override EventMeshCommands Command => EventMeshCommands.HELLO_REQUEST;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(ClientId);
            context.WriteString(ClientSecret);
            context.WriteString(QueueName);
            context.WriteInteger((int)Purpose);
        }

        public HelloRequest Extract(ReadBufferContext context)
        {
            ClientId = context.NextString();
            ClientSecret = context.NextString();
            QueueName = context.NextString();
            Purpose = (ClientPurposeTypes)context.NextInt();
            return this;
        }
    }
}
