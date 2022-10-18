using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetApplicationDomainRequest : BaseEventMeshPackage
    {
        public GetApplicationDomainRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_APPLICATION_DOMAIN_REQUEST;
        public string Name { get; set; }
        public string Vpn { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Name);
            context.WriteString(Vpn);
        }

        public GetApplicationDomainRequest Extract(ReadBufferContext context)
        {
            Name = context.NextString();
            Vpn = context.NextString();
            return this;
        }
    }
}
