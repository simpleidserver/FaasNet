using System.Collections.Generic;
using System.Linq;

namespace EventMesh.Runtime.Messages
{
    public class SubscriptionRequest : Package
    {
        public SubscriptionRequest()
        {
            Topics = new List<SubscriptionItem>();
        }

        public ICollection<SubscriptionItem> Topics { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteInteger(Topics.Count());
            foreach(var topic in Topics)
            {
                topic.Serialize(context);
            }
        }

        public void Extract(ReadBufferContext context)
        {
            int nbTopics = context.NextInt();
            for(int i = 0; i < nbTopics; i++)
            {
                Topics.Add(SubscriptionItem.Deserialize(context));
            }
        }
    }
}
