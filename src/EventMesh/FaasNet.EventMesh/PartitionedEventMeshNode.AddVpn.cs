using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddVpnRequest addVpnRequest, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(VPN_PARTITION_KEY, new GetVpnQuery { Id = addVpnRequest.Vpn}, cancellationToken);
            if (vpn.Success) return PackageResponseBuilder.AddVpn(addVpnRequest.Seq, AddVpnErrorStatus.EXISTINGVPN);
            var addVpnCommand = new AddVpnCommand { Id = addVpnRequest.Vpn, Description = addVpnRequest.Description };
            var result = await Send(VPN_PARTITION_KEY, addVpnCommand, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.AddVpn(addVpnRequest.Seq, AddVpnErrorStatus.NOLEADER);
            return PackageResponseBuilder.AddVpn(addVpnRequest.Seq);
        }
    }
}
