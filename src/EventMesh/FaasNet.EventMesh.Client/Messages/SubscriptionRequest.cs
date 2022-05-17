using FaasNet.RaftConsensus.Client.Messages;
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

        public string SessionId { get; set; }
        public string GroupId { get; set; }
        public ICollection<SubscriptionItem> TopicFilters { get; set; }

        #endregion

        public override void Serialize(WriteBufferContext context)
        {
            base.Serialize(context);
            context.WriteString(SessionId); 
            context.WriteString(GroupId);
            context.WriteInteger(TopicFilters.Count());
            foreach(var topic in TopicFilters)
            {
                topic.Serialize(context);
            }
        }

        public void Extract(ReadBufferContext context)
        {
            SessionId = context.NextString();
            GroupId = context.NextString();
            int nbTopics = context.NextInt();
            for(int i = 0; i < nbTopics; i++)
            {
                TopicFilters.Add(SubscriptionItem.Deserialize(context));
            }
        }
    }

    public class SubscriptionItem
    {
        public string Topic { get; set; }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Topic);
        }

        public static SubscriptionItem Deserialize(ReadBufferContext context)
        {
            return new SubscriptionItem
            {
                Topic = context.NextString()
            };
        }
    }
}
