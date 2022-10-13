using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class RemoveLinkEventDefinitionRequest : BaseEventMeshPackage
    {
        public RemoveLinkEventDefinitionRequest(string seq) : base(seq)
        {
        }

        public string Id { get; set; }
        public string Vpn { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }

        public override EventMeshCommands Command => EventMeshCommands.REMOVE_LINK_EVENT_DEFINITION_REQUEST;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteString(Vpn);
            context.WriteString(Source);
            context.WriteString(Target);
        }

        public RemoveLinkEventDefinitionRequest Extract(ReadBufferContext context)
        {
            Id = context.NextString();
            Vpn = context.NextString();
            Source = context.NextString();
            Target = context.NextString();
            return this;
        }
    }
}
