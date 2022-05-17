using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class ReadMessageTopicResponse : Package
    {
        public string Topic { get; set; }
        public int EvtOffset { get; set; }
        public bool ContainsMessage { get; set; }
        public CloudEvent Value { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Topic);
            context.WriteInteger(EvtOffset);
            context.WriteBoolean(ContainsMessage);
            if (!ContainsMessage) return;
            Value.Serialize(context);
        }

        public void Extract(ReadBufferContext context)
        {
            Topic = context.NextString();
            EvtOffset = context.NextInt();
            ContainsMessage = context.NextBoolean();
            if (ContainsMessage) Value = context.DeserializeCloudEvent();
        }
    }
}
