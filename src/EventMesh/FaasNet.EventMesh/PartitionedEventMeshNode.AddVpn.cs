using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddVpnRequest addVpnRequest, CancellationToken cancellationToken)
        {
            var vpnCollection = await GetStateMachine<VpnCollection>(VPN_PARTITION_KEY, cancellationToken);
            if (vpnCollection.Values.Any(v => v.Id == addVpnRequest.Vpn)) return PackageResponseBuilder.AddVpn(addVpnRequest.Seq, AddVpnErrorStatus.EXISTINGVPN);
            var addVpnCommand = new AddVpnRecordCommand { Record = new VpnRecord { Id = addVpnRequest.Vpn, Description = addVpnRequest.Description } };
            await Send(VPN_PARTITION_KEY, addVpnCommand, cancellationToken);
            return PackageResponseBuilder.AddVpn(addVpnRequest.Seq);
        }
    }
}
