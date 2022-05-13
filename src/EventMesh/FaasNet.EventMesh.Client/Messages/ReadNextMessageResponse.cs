using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class ReadNextMessageResponse : Package
    {
        public bool ContainsMessage { get; set; }
        public CloudEvent CloudEvt { get; set; }
        public int EvtOffset { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteBoolean(ContainsMessage);
            if (!ContainsMessage) return;
            CloudEvt.Serialize(context);
            context.WriteInteger(EvtOffset);
        }

        public void Extract(ReadBufferContext context)
        {
            ContainsMessage = context.NextBoolean();
            if (!ContainsMessage) return;
            CloudEvt = context.DeserializeCloudEvent();
            EvtOffset = context.NextInt();
        }
    }
}
