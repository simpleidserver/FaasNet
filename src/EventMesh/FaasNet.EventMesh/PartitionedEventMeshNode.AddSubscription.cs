using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Queue;
using FaasNet.EventMesh.Client.StateMachines.Subscription;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddSubscriptionRequest addSubscriptionRequest, CancellationToken cancellationToken)
        {
            var subscriptionId = Guid.NewGuid().ToString();
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = addSubscriptionRequest.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.AddSubscription(addSubscriptionRequest.Seq, AddSubscriptionStatus.UNKNOWN_VPN);
            var queue = await Query<GetQueueQueryResult>(PartitionNames.QUEUE_PARTITION_KEY, new GetQueueQuery { QueueName = addSubscriptionRequest.QueueName, Vpn = addSubscriptionRequest.Vpn }, cancellationToken);
            if (!queue.Success) return PackageResponseBuilder.AddSubscription(addSubscriptionRequest.Seq, AddSubscriptionStatus.UNKNOWN_QUEUE);
            await Send(PartitionNames.SUBSCRIPTION_PARTITION_KEY, new AddSubscriptionCommand { Id = subscriptionId, QueueName = addSubscriptionRequest.QueueName, Topic = addSubscriptionRequest.Topic, Vpn = addSubscriptionRequest.Vpn }, cancellationToken);
            return PackageResponseBuilder.AddSubscription(addSubscriptionRequest.Seq, subscriptionId);
        }
    }
}
