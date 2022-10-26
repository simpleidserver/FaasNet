using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using System.Collections.Generic;

namespace FaasNet.EventMesh.Client.StateMachines.Subscription
{
    public class GetAllSubscriptionsQueryResult : IQueryResult
    {
        public ICollection<SubscriptionResult> Subscriptions { get; set; } = new List<SubscriptionResult>();

        public void Deserialize(ReadBufferContext context)
        {
            var nb = context.NextInt();
            for(var i = 0; i < nb; i++)
            {
                var record = new SubscriptionResult();
                record.Deserialize(context);
                Subscriptions.Add(record);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteInteger(Subscriptions.Count);
            foreach (var subscription in Subscriptions)
                subscription.Serialize(context);
        }
    }
}
