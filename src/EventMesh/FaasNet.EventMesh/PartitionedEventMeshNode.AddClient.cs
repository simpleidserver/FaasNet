using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddClientRequest addClientRequest, CancellationToken cancellationToken)
        {
            var vpnCollection = await GetStateMachine<VpnCollection>(VPN_PARTITION_KEY, cancellationToken);
            if (!vpnCollection.Values.Any(v => v.Id == addClientRequest.Vpn)) return PackageResponseBuilder.AddClient(addClientRequest.Seq, AddClientErrorStatus.UNKNOWNVPN);
            var clientCollection = await GetStateMachine<ClientCollection>(CLIENT_PARTITION_KEY, cancellationToken);
            if (clientCollection.Values.Any(c => c.Id == addClientRequest.Id)) return PackageResponseBuilder.AddClient(addClientRequest.Seq, AddClientErrorStatus.EXISTINGCLIENT);
            var addClientCommand = new AddClientRecordCommand { Record = new ClientRecord { Id = addClientRequest.Id, Vpn = addClientRequest.Vpn, Purposes = addClientRequest.Purposes } };
            await Send(CLIENT_PARTITION_KEY, addClientCommand, cancellationToken);
            return PackageResponseBuilder.AddClient(addClientRequest.Seq);
        }
    }
}
