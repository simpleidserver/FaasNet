using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.EventMesh.Client.StateMachines.Subscription;
using FaasNet.EventMesh.StateMachines.ApplicationDomain;
using FaasNet.Partition;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.StateMachines.Subscriptions
{
    public class SubscriptionConsumer : BaseIntegrationEventConsumer, IConsumer<ApplicationDomainLinkAdded>, IConsumer<ApplicationDomainLinkRemoved>
    {
        public SubscriptionConsumer(IPartitionCluster partitionCluster) : base(partitionCluster)
        {
        }

        public async Task Consume(ApplicationDomainLinkAdded request, CancellationToken cancellationToken)
        {
            var evt = await Query<GetEventDefinitionQueryResult>(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new GetEventDefinitionQuery { Id = request.EventId, Vpn = request.Vpn }, cancellationToken);
            if (!evt.Success) return;
            await Send(PartitionNames.SUBSCRIPTION_PARTITION_KEY, new AddSubscriptionCommand { ClientId = request.Target, EventId = request.EventId, Topic = evt.EventDef.Topic, Vpn = request.Vpn, Id = Guid.NewGuid().ToString() }, cancellationToken);
        }

        public async Task Consume(ApplicationDomainLinkRemoved request, CancellationToken cancellationToken)
        {
            await Send(PartitionNames.SUBSCRIPTION_PARTITION_KEY, new RemoveSubscriptionCommand { ClientId = request.Target, EventId = request.EventId, Vpn = request.Vpn }, cancellationToken);
        }
    }
}
