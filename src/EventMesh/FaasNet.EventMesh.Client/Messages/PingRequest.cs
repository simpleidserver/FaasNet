using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class PingRequest : BaseEventMeshPackage
    {
        public PingRequest(string seq) : base(seq) { }

        public override EventMeshCommands Command => EventMeshCommands.HEARTBEAT_REQUEST;

        protected override void SerializeAction(WriteBufferContext context) { }
    }
}
