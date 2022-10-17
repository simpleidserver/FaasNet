using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.ApplicationDomain
{
    public class GetAllApplicationDomainsQuery : IQuery
    {
        public FilterQuery Filter { get; set; } = new FilterQuery
        {
            SortBy = nameof(ApplicationDomainQueryResult.CreateDateTime),
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
