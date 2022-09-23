using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Session
{
    public class SearchSessionsQuery : IQuery
    {
        public string ClientId { get; set; }
        public string Vpn { get; set; }
        public FilterQuery Filter { get; set; } = new FilterQuery
        {
            SortBy = nameof(SessionQueryResult.ExpirationTime),
            SortOrder = SortOrders.DESC
        };

        public void Deserialize(ReadBufferContext context)
        {
            ClientId = context.NextString();
            Vpn = context.NextString();
            Filter = new FilterQuery();
            Filter.Deserialize(context);
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(ClientId);
            context.WriteString(Vpn);
            Filter.Serialize(context);
        }
    }
}
