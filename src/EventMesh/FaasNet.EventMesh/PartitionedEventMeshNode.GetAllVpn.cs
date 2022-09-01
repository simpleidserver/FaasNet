using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetAllVpnRequest request, CancellationToken cancellationToken)
        {
            var vpnCollection = await GetStateMachine<VpnCollection>(VPN_PARTITION_KEY, cancellationToken);
            return PackageResponseBuilder.GetAllVpn(request.Seq, vpnCollection.Values.Select(v => new VpnResult
            {
                Description = v.Description,
                Id = v.Id
            }).ToList());
        }
    }
}
