using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class PublishMessageRequest : Package
    {
        public string SessionId { get; set; }
        public string Topic { get; set; }
        public CloudEvent CloudEvent { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(SessionId);
            context.WriteString(Topic);
            CloudEvent.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            SessionId = context.NextString();
            Topic = context.NextString();
            CloudEvent = context.DeserializeCloudEvent();
        }
    }
}
