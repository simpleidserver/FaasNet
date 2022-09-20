using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Client;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetAllClientRequest getAllClientRequest, CancellationToken cancellationToken)
        {
            var res = await Query<GenericSearchQueryResult<ClientQueryResult>>(CLIENT_PARTITION_KEY, new GetAllClientsQuery { Filter = getAllClientRequest.Filter }, cancellationToken);
            return PackageResponseBuilder.GetAllClient(getAllClientRequest.Seq, res);
        }
    }
}
