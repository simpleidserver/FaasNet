using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetAllVpnRequest request, CancellationToken cancellationToken)
        {
            var res = await Query<GenericSearchQueryResult<VpnQueryResult>>(PartitionNames.VPN_PARTITION_KEY, new GetAllVpnQuery { Filter = request.Filter }, cancellationToken);
            return PackageResponseBuilder.GetAllVpn(request.Seq, res);
        }
    }
}
