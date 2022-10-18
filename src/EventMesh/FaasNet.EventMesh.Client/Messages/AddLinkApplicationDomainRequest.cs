using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddLinkApplicationDomainRequest : BaseEventMeshPackage
    {
        public AddLinkApplicationDomainRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_LINK_APPLICATION_DOMAIN_REQUEST;
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string EventId { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteString(Source);
            context.WriteString(Target);
            context.WriteString(EventId);
        }

        public AddLinkApplicationDomainRequest Extract(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            Source = context.NextString();
            Target = context.NextString();
            EventId = context.NextString();
            return this;
        }
    }
}
