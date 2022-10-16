using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(FindVpnsByNameRequest request, CancellationToken cancellationToken)
        {
            var res = await Query<FindVpnsByNameQueryResult>(PartitionNames.VPN_PARTITION_KEY, new FindVpnsByNameQuery { Name = request.Name }, cancellationToken);
            return PackageResponseBuilder.FindVpnsByName(request.Seq, res.Content);
        }
    }
}
