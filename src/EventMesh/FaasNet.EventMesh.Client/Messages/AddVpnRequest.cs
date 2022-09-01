using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddVpnRequest : BaseEventMeshPackage
    {
        public AddVpnRequest(string seq) : base(seq) { }

        public AddVpnRequest(string seq, string vpn, string description) : this(seq)
        {
            Vpn = vpn;
            Description = description;
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_VPN_REQUEST;
        public string Vpn { get; private set; }
        public string Description { get; private set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Vpn);
            context.WriteString(Description);
        }

        public AddVpnRequest Extract(ReadBufferContext context)
        {
            Vpn = context.NextString();
            Description = context.NextString();
            return this;
        }
    }
}
