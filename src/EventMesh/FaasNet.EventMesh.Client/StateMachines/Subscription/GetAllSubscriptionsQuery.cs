using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Subscription
{
    public class GetAllSubscriptionsQuery : IQuery
    {
        public string TopicFilter { get; set; }
        public string Vpn { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            TopicFilter = context.NextString();
            Vpn = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(TopicFilter);
            context.WriteString(Vpn);
        }
    }
}
