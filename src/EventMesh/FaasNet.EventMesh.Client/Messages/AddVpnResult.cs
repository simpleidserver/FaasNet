using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddVpnResult : BaseEventMeshPackage
    {
        public AddVpnResult(string seq) : base(seq)
        {
            Success = true;
        }

        public AddVpnResult(string seq, AddVpnErrorStatus status) : this(seq)
        {
            Success = false;
            Status = status;
        }

        public bool Success { get; private set; }
        public AddVpnErrorStatus? Status { get; private set; }

        public override EventMeshCommands Command => EventMeshCommands.ADD_VPN_RESPONSE;

        protected override void SerializeAction(WriteBufferContext context) 
        {
            context.WriteBoolean(Success);
            if (!Success) context.WriteInteger((int)Status);
        }

        public AddVpnResult Extract(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if (!Success) Status = (AddVpnErrorStatus)context.NextInt();
            return this;
        }
    }

    public enum AddVpnErrorStatus
    {
        EXISTINGVPN = 0,
        NOLEADER = 1
    }
}
