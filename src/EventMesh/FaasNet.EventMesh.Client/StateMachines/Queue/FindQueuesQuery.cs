using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Queue
{
    public class FindQueuesQuery : IQuery
    {
        public FilterQuery Filter { get; set; } = new FilterQuery
        {
            SortBy = nameof(QueueQueryResult.TopicFilter),
            SortOrder = SortOrders.ASC
        };

        public void Deserialize(ReadBufferContext context)
        {
            Filter = new FilterQuery();
            Filter.Deserialize(context);
        }

        public void Serialize(WriteBufferContext context)
        {
            Filter.Serialize(context);
        }
    }
}
