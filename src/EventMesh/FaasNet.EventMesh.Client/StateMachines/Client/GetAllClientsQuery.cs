using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Client
{
    public class GetAllClientsQuery : IQuery
    {
        public FilterQuery Filter { get; set; } = new FilterQuery
        {
            SortBy = nameof(ClientQueryResult.CreateDateTime),
            SortOrder = SortOrders.DESC
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
