using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllVpnRequest : BaseEventMeshPackage
    {
        public GetAllVpnRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_ALL_VPN_REQUEST;

        protected override void SerializeAction(WriteBufferContext context) { }
    }
}
