using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetClientRequest getClientRequest, CancellationToken cancellationToken)
        {
            var result = await Query<GetClientQueryResult>(CLIENT_PARTITION_KEY, new GetClientQuery { Id = getClientRequest.ClientId, Vpn = getClientRequest.Vpn }, cancellationToken);
            return PackageResponseBuilder.GetClient(getClientRequest.Seq, result.Success, result.Client);
        }
    }
}