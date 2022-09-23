﻿using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.EventMesh.Client.StateMachines.Queue;
using FaasNet.EventMesh.Client.StateMachines.QueueMessage;
using FaasNet.EventMesh.Client.StateMachines.Session;
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
            var session = await Query<GetSessionQueryResult>(SESSION_PARTITION_KEY, new GetSessionQuery { Id = request.SessionId }, cancellationToken);
            if (!session.Success) return PackageResponseBuilder.PublishMessage(request.Seq, PublishMessageStatus.UNKNOWN_SESSION);
            if (!session.Session.IsValid) return PackageResponseBuilder.PublishMessage(request.Seq, PublishMessageStatus.EXPIRED_SESSION);
            if (session.Session.ClientPurpose != ClientPurposeTypes.PUBLISH) return PackageResponseBuilder.PublishMessage(request.Seq, PublishMessageStatus.BAD_SESSION_USAGE);
            var allQueues = await Query<SearchQueuesQueryResult>(QUEUE_PARTITION_KEY, new SearchQueuesQuery { Vpn = session.Session.Vpn, TopicMessage = request.Topic }, cancellationToken);
            var filteredQueues = allQueues;
            var publishedQueueNames = new ConcurrentBag<string>();
            var id = Guid.NewGuid().ToString();
            await Parallel.ForEachAsync(filteredQueues.Queues, new ParallelOptions
            {
                MaxDegreeOfParallelism = _eventMeshOptions.MaxNbThreads
            }, async (q, t) =>
            {
                var partitionKey = $"{q.Vpn}_{q.QueueName}";
                var addMessageCommand = new AddQueueMessageCommand { Id = id, Data = request.CloudEvent, Topic = request.Topic };
                var result = await Send(partitionKey, addMessageCommand, cancellationToken);
                if (result.Success) publishedQueueNames.Add(partitionKey);
            });

            return PackageResponseBuilder.PublishMessage(request.Seq, id, publishedQueueNames.ToArray());
        }
    }
}
