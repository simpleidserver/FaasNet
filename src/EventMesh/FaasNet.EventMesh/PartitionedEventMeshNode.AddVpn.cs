using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddVpnRequest addVpnRequest, CancellationToken cancellationToken)
        {
            var vpn = await GetStateMachine<VpnStateMachine>(VPN_PARTITION_KEY, addVpnRequest.Vpn, cancellationToken);
            if (vpn != null) return PackageResponseBuilder.AddVpn(addVpnRequest.Seq, AddVpnErrorStatus.EXISTINGVPN);
            var addVpnCommand = new AddVpnCommand { Description = addVpnRequest.Description };
            await Send(VPN_PARTITION_KEY, addVpnRequest.Vpn, addVpnCommand, cancellationToken);
            return PackageResponseBuilder.AddVpn(addVpnRequest.Seq);
        }
    }
}
