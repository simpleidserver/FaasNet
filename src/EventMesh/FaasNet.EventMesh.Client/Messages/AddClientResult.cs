using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddClientResult : BaseEventMeshPackage
    {
        public AddClientResult(string seq) : base(seq)
        {
            Success = true;
        }

        public AddClientResult(string seq, AddClientErrorStatus status) : this(seq)
        {
            Success = false;
            Status = status;
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_CLIENT_RESPONSE;
        public bool Success { get; private set; }
        public AddClientErrorStatus? Status { get; private set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
            if (!Success) context.WriteInteger((int)Status);
            else
            {
                context.WriteString(ClientId);
                context.WriteString(ClientSecret);
            }
        }

        public AddClientResult Extract(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if (!Success) Status = (AddClientErrorStatus)context.NextInt();
            else
            {
                ClientId = context.NextString();
                ClientSecret = context.NextString();
            }

            return this;
        }
    }

    public enum AddClientErrorStatus
    {
        UNKNOWN_VPN = 0,
        EXISTING_CLIENT = 1
    }
}
