using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class RemoveClientResult : BaseEventMeshPackage
    {
        public RemoveClientResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.REMOVE_CLIENT_RESPONSE;
        public RemoveClientStatus Status { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
        }

        public RemoveClientResult Extract(ReadBufferContext context)
        {
            Status = (RemoveClientStatus)context.NextInt();
            return this;
        }
    }

    public enum RemoveClientStatus
    {
        OK = 0,
        UNKNOWN_CLIENT = 1,
        NOLEADER = 2
    }
}
