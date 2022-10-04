using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class BulkUpdateClientResult : BaseEventMeshPackage
    {
        public BulkUpdateClientResult(string seq) : base(seq)
        {
            Success = true;
        }

        public BulkUpdateClientResult(string seq, UpdateClientErrorStatus status) : this(seq)
        {
            Success = false;
            Status = status;
        }

        public override EventMeshCommands Command => EventMeshCommands.BULK_UPDATE_CLIENT_RESPONSE;
        public bool Success { get; private set; }
        public UpdateClientErrorStatus? Status { get; private set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
            if (!Success) context.WriteInteger((int)Status);
        }

        public BulkUpdateClientResult Extract(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if (!Success) Status = (UpdateClientErrorStatus)context.NextInt();
            return this;
        }
    }

    public enum UpdateClientErrorStatus
    {
        UNKNOWN_VPN = 0,
        NOLEADER = 1
    }
}
