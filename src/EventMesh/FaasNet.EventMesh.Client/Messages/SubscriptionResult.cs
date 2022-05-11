using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.Messages
{
    public class SubscriptionResult : Package
    {
        public IEnumerable<string> QueueNames { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteStringArray(QueueNames);
        }

        public void Extract(ReadBufferContext context)
        {
            QueueNames = context.NextStringArray();
        }
    }
}
