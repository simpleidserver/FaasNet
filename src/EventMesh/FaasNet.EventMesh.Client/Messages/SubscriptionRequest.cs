using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Client.Messages
{
    public class SubscriptionRequest : Package
    {
        public SubscriptionRequest()
        {
            TopicFilters = new List<SubscriptionItem>();
        }

        #region Properties

        public string ClientId { get; set; }
        public string SessionId { get; set; }
        public ICollection<SubscriptionItem> TopicFilters { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(ClientId);
            context.WriteString(SessionId);
            context.WriteInteger(TopicFilters.Count());
            foreach(var topic in TopicFilters)
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
                TopicFilters.Add(SubscriptionItem.Deserialize(context));
            }
        }
    }
}
