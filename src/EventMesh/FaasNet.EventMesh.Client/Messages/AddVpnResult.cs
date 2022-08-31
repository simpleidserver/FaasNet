using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddVpnResult : BaseEventMeshPackage
    {
        public AddVpnResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_VPN_RESPONSE;

        protected override void SerializeAction(WriteBufferContext context) { }
    }
}
