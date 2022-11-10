using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.EventMesh.StateMachines.ApplicationDomain;
using FaasNet.Partition;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.EventDefinition
{
    public class EventDefinitionConsumer : BaseIntegrationEventConsumer, IConsumer<ApplicationDomainLinkAdded>, IConsumer<ApplicationDomainLinkRemoved>
    {
        private readonly EventMeshOptions _options;

        public EventDefinitionConsumer(IOptions<EventMeshOptions> options, IPartitionCluster partitionCluster) : base(partitionCluster)
        {
            _options = options.Value;
        }

        public void Dispose()
        {
        }

        public async Task Consume(ApplicationDomainLinkAdded message, CancellationToken cancellationToken)
        {
            await Send(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new AddLinkEventDefinitionCommand
            {
                Id = message.EventId,
                Source = message.Source,
                Target= message.Target,
                Vpn = message.Vpn
            }, CancellationToken.None);
        }

        public async Task Consume(ApplicationDomainLinkRemoved message, CancellationToken cancellationToken)
        {
            await Send(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new RemoveLinkEventDefinitionCommand
            {
                Id = message.EventId,
                Source = message.Source,
                Target = message.Target,
                Vpn = message.Vpn
            }, CancellationToken.None);
        }
    }
}
