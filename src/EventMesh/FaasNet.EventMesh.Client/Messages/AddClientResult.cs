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
        public long Term { get; set; }
        public long MatchIndex { get; set; }
        public long LastIndex { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
            if (!Success) context.WriteInteger((int)Status);
            else
            {
                context.WriteString(ClientId);
                context.WriteString(ClientSecret);
                context.WriteLong(Term);
                context.WriteLong(MatchIndex);
                context.WriteLong(LastIndex);
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
                Term = context.NextLong();
                MatchIndex = context.NextLong();
                LastIndex = context.NextLong();
            }

            return this;
        }
    }

    public enum AddClientErrorStatus
    {
        UNKNOWN_VPN = 0,
        EXISTING_CLIENT = 1,
        NOLEADER = 2
    }
}
