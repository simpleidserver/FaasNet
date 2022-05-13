using FaasNet.RaftConsensus.Client.Messages;

namespace FaasNet.EventMesh.Client.Messages
{
    public class SubscriptionResult : Package
    {
        public string QueueName { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(QueueName);
        }

        public void Extract(ReadBufferContext context)
        {
            QueueName = context.NextString();
        }
    }
}
