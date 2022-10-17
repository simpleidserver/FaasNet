using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddApplicationDomainRequest : BaseEventMeshPackage
    {
        public AddApplicationDomainRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_APPLICATION_DOMAIN_REQUEST;
        public string Name { get; set; }
        public string Vpn { get; set; }
        public string Description { get; set; }
        public string RootTopic { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
            context.WriteString(Description);
            context.WriteString(RootTopic);
        }

        public AddApplicationDomainRequest Extract(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            Description = context.NextString();
            RootTopic = context.NextString();
            return this;
        }
    }
}
