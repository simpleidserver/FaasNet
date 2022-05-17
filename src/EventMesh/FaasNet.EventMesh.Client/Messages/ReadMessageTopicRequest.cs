using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class ReadMessageTopicRequest : Package
    {
        public string Topic { get; set; }
        public int EvtOffset { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(Topic);
            context.WriteInteger(EvtOffset);
        }

        public void Extract(ReadBufferContext context)
        {
            Topic = context.NextString();
            EvtOffset = context.NextInt();
        }
    }
}
