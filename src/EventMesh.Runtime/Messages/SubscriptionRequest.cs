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

        #region Properties

        public string ClientId { get; set; }
        public string SessionId { get; set; }
        public ICollection<SubscriptionItem> Topics { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(ClientId);
            context.WriteString(SessionId);
            context.WriteInteger(Topics.Count());
            foreach(var topic in Topics)
            {
                topic.Serialize(context);
            }
        }

        public void Extract(ReadBufferContext context)
        {
            ClientId = context.NextString();
            SessionId = context.NextString();
            int nbTopics = context.NextInt();
            for(int i = 0; i < nbTopics; i++)
            {
                Topics.Add(SubscriptionItem.Deserialize(context));
            }
        }
    }
}
