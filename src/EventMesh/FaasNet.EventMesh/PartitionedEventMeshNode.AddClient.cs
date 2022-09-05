using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddClientRequest addClientRequest, CancellationToken cancellationToken)
        {
            var vpn = await GetStateMachine<VpnStateMachine>(VPN_PARTITION_KEY, addClientRequest.Vpn, cancellationToken);
            if (vpn == null) return PackageResponseBuilder.AddClient(addClientRequest.Seq, AddClientErrorStatus.UNKNOWNVPN);
            var client = await GetStateMachine<ClientStateMachine>(CLIENT_PARTITION_KEY, addClientRequest.Id, cancellationToken);
            if (client != null) return PackageResponseBuilder.AddClient(addClientRequest.Seq, AddClientErrorStatus.EXISTINGCLIENT);
            var addClientCommand = new AddClientCommand { Purposes = addClientRequest.Purposes, Vpn = addClientRequest.Vpn };
            await Send(CLIENT_PARTITION_KEY, addClientRequest.Id,addClientCommand, cancellationToken);
            return PackageResponseBuilder.AddClient(addClientRequest.Seq);
        }
    }
}
