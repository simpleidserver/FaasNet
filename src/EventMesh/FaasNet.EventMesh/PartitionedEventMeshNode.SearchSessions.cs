using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Session;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(SearchSessionsRequest request, CancellationToken cancellationToken)
        {
            var res = await Query<GenericSearchQueryResult<SessionQueryResult>>(PartitionNames.SESSION_PARTITION_KEY, new SearchSessionsQuery 
            { 
                ClientId = request.ClientId, 
                Vpn = request.Vpn, 
                Filter = request.Filter 
            }, cancellationToken);
            return PackageResponseBuilder.SearchSessions(request.Seq, res);
        }
    }
}
