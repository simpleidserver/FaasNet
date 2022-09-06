using CloudNative.CloudEvents;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class ReadMessageResult : BaseEventMeshPackage
    {
        public ReadMessageResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.READ_MESSAGE_RESPONSE;
        public ReadMessageStatus Status { get; set; }
        public CloudEvent Message { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
            if (Status == ReadMessageStatus.SUCCESS) Message.Serialize(context);
        }

        public ReadMessageResult Extract(ReadBufferContext context)
        {
            Status = (ReadMessageStatus)context.NextInt();
            if (Status == ReadMessageStatus.SUCCESS) Message = context.NextCloudEvent();
            return this;
        }
    }

    public enum ReadMessageStatus
    {
        SUCCESS = 0,
        NO_MESSAGE = 1,
        UNKNOWN_SESSION = 2,
        EXPIRED_SESSION = 3,
        BAD_SESSION_USAGE = 4
    }
}
