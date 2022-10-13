using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
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
            await RemoveTargets(removeClientRequest, client.Client, cancellationToken);
            var removeClientCommand = new RemoveClientCommand { ClientId = removeClientRequest.ClientId, Vpn = removeClientRequest.Vpn };
            var result = await Send(CLIENT_PARTITION_KEY, removeClientCommand, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.RemoveClient(removeClientRequest.Seq, RemoveClientStatus.NOLEADER);
            return PackageResponseBuilder.RemoveClient(removeClientRequest.Seq);
        }

        async Task RemoveTargets(RemoveClientRequest removeClientRequest, ClientQueryResult client, CancellationToken cancellationToken)
        {
            await Parallel.ForEachAsync(client.Targets, new ParallelOptions
            {
                MaxDegreeOfParallelism = _eventMeshOptions.MaxNbThreads
            }, async (n, t) =>
            {
                await Send(EVENTDEFINITION_PARTITION_KEY, new RemoveLinkEventDefinitionCommand { Id = n.EventId, Vpn = removeClientRequest.Vpn, Source = client.Id, Target = n.Target }, cancellationToken);
            });
        }
    }
}
