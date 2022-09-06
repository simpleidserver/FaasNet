using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddClientRequest addClientRequest, CancellationToken cancellationToken)
        {
            var vpn = await GetStateMachine<VpnStateMachine>(VPN_PARTITION_KEY, addClientRequest.Vpn, cancellationToken);
            if (vpn == null) return PackageResponseBuilder.AddClient(addClientRequest.Seq, AddClientErrorStatus.UNKNOWN_VPN);
            var client = await GetStateMachine<ClientStateMachine>(CLIENT_PARTITION_KEY, addClientRequest.Id, cancellationToken);
            if (client != null) return PackageResponseBuilder.AddClient(addClientRequest.Seq, AddClientErrorStatus.EXISTING_CLIENT);
            var clientSecret = Guid.NewGuid().ToString();
            var addClientCommand = new AddClientCommand { Purposes = addClientRequest.Purposes, Vpn = addClientRequest.Vpn, Secret = ClientStateMachine.ComputePassword(clientSecret), SessionExpirationTimeMS = _eventMeshOptions.ClientSessionExpirationTimeMS };
            await Send(CLIENT_PARTITION_KEY, addClientRequest.Id,addClientCommand, cancellationToken);
            return PackageResponseBuilder.AddClient(addClientRequest.Seq, addClientRequest.Id, clientSecret);
        }
    }
}
