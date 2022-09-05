using FaasNet.Peer.Client;
using System;

namespace FaasNet.EventMesh.Client.Messages
{
    public class PublishMessageResult : BaseEventMeshPackage
    {
        public PublishMessageResult(string seq) : base(seq)
        {
            Status = PublishMessageStatus.SUCCESS;
        }

        public PublishMessageResult(string seq, PublishMessageStatus status) : this(seq)
        {
            Status = status;
        }

        public override EventMeshCommands Command => EventMeshCommands.PUBLISH_MESSAGE_RESPONSE;
        public PublishMessageStatus Status { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
        }

        public PublishMessageResult Extract(ReadBufferContext context)
        {
            Status = (PublishMessageStatus)context.NextInt();
            return this;
        }
    }

    public enum PublishMessageStatus
    {
        SUCCESS = 0,
        UNKNOWN_TOPIC = 1
    }
}
