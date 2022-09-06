using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class HelloResult : BaseEventMeshPackage
    {
        public HelloResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.HELLO_RESPONSE;
        public HelloMessageStatus Status { get; set; }
        public string SessionId { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
            if (Status == HelloMessageStatus.SUCCESS) context.WriteString(SessionId); 
        }

        public HelloResult Extract(ReadBufferContext context)
        {
            Status = (HelloMessageStatus)context.NextInt();
            if (Status == HelloMessageStatus.SUCCESS) SessionId = context.NextString();
            return this;
        }
    }

    public enum HelloMessageStatus
    {
        SUCCESS = 0,
        UNKNOWN_CLIENT =  1,
        BAD_CREDENTIALS = 2,
        BAD_PURPOSE = 3
    }
}
