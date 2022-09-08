using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.Messages
{
    public class PublishMessageResult : BaseEventMeshPackage
    {
        public PublishMessageResult(string seq) : base(seq)
        {
            Status = PublishMessageStatus.SUCCESS;
            QueueNames = new List<string>();
        }

        public PublishMessageResult(string seq, PublishMessageStatus status) : this(seq)
        {
            Status = status;
        }

        public override EventMeshCommands Command => EventMeshCommands.PUBLISH_MESSAGE_RESPONSE;
        public PublishMessageStatus Status { get; set; }
        public IEnumerable<string> QueueNames { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
            context.WriteInteger(QueueNames.Count());
            foreach(var queueName in QueueNames) context.WriteString(queueName);
        }

        public PublishMessageResult Extract(ReadBufferContext context)
        {
            Status = (PublishMessageStatus)context.NextInt();
            var result = new List<string>();
            var nb = context.NextInt();
            for(var i = 0; i < nb; i++) result.Add(context.NextString());
            QueueNames = result;
            return this;
        }
    }

    public enum PublishMessageStatus
    {
        SUCCESS = 0,
        UNKNOWN_TOPIC = 1,
        UNKNOWN_SESSION = 2,
        EXPIRED_SESSION = 3,
        BAD_SESSION_USAGE = 4
    }
}
