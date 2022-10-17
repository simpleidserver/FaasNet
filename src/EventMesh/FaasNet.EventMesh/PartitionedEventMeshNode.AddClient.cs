using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddClientRequest addClientRequest, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = addClientRequest.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.AddClient(addClientRequest.Seq, AddClientErrorStatus.UNKNOWN_VPN);
            var client = await Query<GetClientQueryResult>(PartitionNames.CLIENT_PARTITION_KEY, new GetClientQuery { Id = addClientRequest.Id, Vpn = addClientRequest.Vpn }, cancellationToken);
            if (client.Success) return PackageResponseBuilder.AddClient(addClientRequest.Seq, AddClientErrorStatus.EXISTING_CLIENT);
            var clientSecret = Guid.NewGuid().ToString();
            var addClientCommand = new AddClientCommand { Id = addClientRequest.Id, Purposes = addClientRequest.Purposes, Vpn = addClientRequest.Vpn, ClientSecret = PasswordHelper.ComputePassword(clientSecret), SessionExpirationTimeMS = _eventMeshOptions.ClientSessionExpirationTimeMS };
            var result = await Send(PartitionNames.CLIENT_PARTITION_KEY, addClientCommand, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.AddClient(addClientRequest.Seq, AddClientErrorStatus.NOLEADER);
            return PackageResponseBuilder.AddClient(addClientRequest.Seq, addClientRequest.Id, clientSecret, result.Term, result.MatchIndex, result.LastIndex);
        }
    }
}
