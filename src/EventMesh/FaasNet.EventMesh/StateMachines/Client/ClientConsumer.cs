using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.StateMachines.EventDefinition;
using FaasNet.Partition;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Client
{
    public class ClientConsumer : BaseIntegrationEventConsumer, IConsumer<EventDefinitionLinkAdded>, IConsumer<EventDefinitionLinkRemoved>
    {
        public ClientConsumer(IPartitionCluster partitionCluster) : base(partitionCluster)
        {

        }

        public async Task Consume(EventDefinitionLinkAdded request, CancellationToken cancellationToken)
        {
            var addTargetCmd = new AddTargetCommand
            {
                ClientId = request.Source,
                Vpn = request.Vpn,
                Target = request.Target,
                EventDefId = request.EventId
            };
            await Send(PartitionNames.CLIENT_PARTITION_KEY, addTargetCmd, cancellationToken);
        }

        public async Task Consume(EventDefinitionLinkRemoved request, CancellationToken cancellationToken)
        {
            var removedTargetCmd = new RemoveTargetCommand
            {
                ClientId = request.Source,
                Vpn = request.Vpn,
                Target = request.Target,
                EventId = request.EventId
            };
            await Send(PartitionNames.CLIENT_PARTITION_KEY, removedTargetCmd, cancellationToken);
        }
    }
}
