using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class RemoveElementApplicationDomainRequest : BaseEventMeshPackage
    {
        public RemoveElementApplicationDomainRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.REMOVE_ELEMENT_APPLICATION_DOMAIN_REQUEST;
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string ElementId { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteString(ElementId);
        }

        public RemoveElementApplicationDomainRequest Extract(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            ElementId = context.NextString();
            return this;
        }
    }
}
