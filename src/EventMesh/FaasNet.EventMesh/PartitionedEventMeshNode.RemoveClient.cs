using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(RemoveClientRequest removeClientRequest, CancellationToken cancellationToken)
        {
            var client = await Query<GetClientQueryResult>(CLIENT_PARTITION_KEY, new GetClientQuery { Id = removeClientRequest.ClientId, Vpn = removeClientRequest.Vpn }, cancellationToken);
            if (!client.Success) return PackageResponseBuilder.RemoveClient(removeClientRequest.Seq, RemoveClientStatus.UNKNOWN_CLIENT);
            var removeClientCommand = new RemoveClientCommand { ClientId = removeClientRequest.ClientId, Vpn = removeClientRequest.Vpn };
            var result = await Send(CLIENT_PARTITION_KEY, removeClientCommand, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.RemoveClient(removeClientRequest.Seq, RemoveClientStatus.NOLEADER);
            return PackageResponseBuilder.RemoveClient(removeClientRequest.Seq);
        }
    }
}
