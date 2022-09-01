using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllClientRequest : BaseEventMeshPackage
    {
        public GetAllClientRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_ALL_CLIENT_REQUEST;

        protected override void SerializeAction(WriteBufferContext context)
        {
        }
    }
}
