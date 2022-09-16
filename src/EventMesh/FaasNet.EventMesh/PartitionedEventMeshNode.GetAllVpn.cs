using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetAllVpnRequest request, CancellationToken cancellationToken)
        {
            var result = await Query<GetAllVpnQueryResult>(VPN_PARTITION_KEY, new GetAllVpnQuery(), cancellationToken);
            return PackageResponseBuilder.GetAllVpn(request.Seq, result.Vpns.Select(v => new VpnResult
            {
                Description = v.Description,
                Id = v.Name
            }).ToList());
        }
    }
}
