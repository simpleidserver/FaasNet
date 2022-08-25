using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class PingResult : BaseEventMeshPackage
    {
        public PingResult(string seq) : base(seq) { }

        public override EventMeshCommands Command => EventMeshCommands.HEARTBEAT_RESPONSE;

        protected override void SerializeAction(WriteBufferContext context) { }
    }
}
