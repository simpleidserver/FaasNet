using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class RemoveLinkApplicationDomain : BaseEventMeshPackage
    {
        public RemoveLinkApplicationDomain(string seq) : base(seq)
        {
        }

        public string Name { get; set; }
        public string Vpn { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
        public string EventId { get; set; }

        public override EventMeshCommands Command => EventMeshCommands.REMOVE_LINK_APPLICATION_DOMAIN_REQUEST;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteString(Source);
            context.WriteString(Target);
            context.WriteString(EventId);
        }

        public RemoveLinkApplicationDomain Extract(ReadBufferContext context)
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
