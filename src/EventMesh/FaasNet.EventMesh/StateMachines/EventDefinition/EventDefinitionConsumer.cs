using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.EventMesh.StateMachines.Client;
using FaasNet.Partition;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.EventDefinition
{
    public class EventDefinitionConsumer : BaseIntegrationEventConsumer, IConsumer<ClientRemoved>
    {
        private readonly EventMeshOptions _options;

        public EventDefinitionConsumer(IOptions<EventMeshOptions> options, IPartitionCluster partitionCluster) : base(partitionCluster)
        {
            _options = options.Value;
        }

        public async Task Consume(ClientRemoved message, CancellationToken cancellationToken)
        {
            await Parallel.ForEachAsync(message.Targets, new ParallelOptions
            {
                MaxDegreeOfParallelism = _options.MaxNbThreads
            }, async (n, t) =>
            {                
                await Send(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new RemoveLinkEventDefinitionCommand { Id = n.EventId, Vpn = message.Vpn, Source = message.Id, Target = n.Target }, CancellationToken.None);
            });
        }
    }
}
