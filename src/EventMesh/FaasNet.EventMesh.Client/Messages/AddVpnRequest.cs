using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddVpnRequest : BaseEventMeshPackage
    {
        public AddVpnRequest(string seq) : base(seq) { }

        public AddVpnRequest(string seq, string vpn) : this(seq)
        {
            Vpn = vpn;
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_VPN_REQUEST;
        public string Vpn { get; private set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Vpn);
        }

        public AddVpnRequest Extract(ReadBufferContext context)
        {
            Vpn = context.NextString();
            return this;
        }
    }
}
