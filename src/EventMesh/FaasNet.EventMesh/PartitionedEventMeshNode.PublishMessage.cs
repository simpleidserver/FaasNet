using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.QueueMessage;
using FaasNet.EventMesh.Client.StateMachines.Session;
using FaasNet.EventMesh.Client.StateMachines.Subscription;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(PublishMessageRequest request, CancellationToken cancellationToken)
        {
            var session = await Query<GetSessionQueryResult>(PartitionNames.SESSION_PARTITION_KEY, new GetSessionQuery { Id = request.SessionId }, cancellationToken);
            if (!session.Success) return PackageResponseBuilder.PublishMessage(request.Seq, PublishMessageStatus.UNKNOWN_SESSION);
            if (!session.Session.IsValid) return PackageResponseBuilder.PublishMessage(request.Seq, PublishMessageStatus.EXPIRED_SESSION);
            if (session.Session.ClientPurpose != ClientPurposeTypes.PUBLISH) return PackageResponseBuilder.PublishMessage(request.Seq, PublishMessageStatus.BAD_SESSION_USAGE);
            var vpn = session.Session.Vpn;
            var allQueues = await Query<GetAllSubscriptionsQueryResult>(PartitionNames.SUBSCRIPTION_PARTITION_KEY, new GetAllSubscriptionsQuery { TopicFilter = request.Topic, Vpn = vpn }, cancellationToken);
            var filteredQueues = allQueues;
            var publishedQueueNames = new ConcurrentBag<string>();
            var id = Guid.NewGuid().ToString();
            await Parallel.ForEachAsync(filteredQueues.Subscriptions, new ParallelOptions
            {
                MaxDegreeOfParallelism = _eventMeshOptions.MaxNbThreads
            }, async (q, t) =>
            {
                var partitionKey = $"{vpn}_{q.QueueName}";
                var addMessageCommand = new AddQueueMessageCommand { Id = id, Data = request.CloudEvent, Topic = request.Topic };
                var result = await Send(partitionKey, addMessageCommand, cancellationToken);
                if (result.Success) publishedQueueNames.Add(partitionKey);
            });

            return PackageResponseBuilder.PublishMessage(request.Seq, id, publishedQueueNames.ToArray());
        }
    }
}
