using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
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
            if (session == null) return PackageResponseBuilder.PublishMessage(request.Seq, PublishMessageStatus.UNKNOWN_SESSION);
            if (!session.IsValid) return PackageResponseBuilder.PublishMessage(request.Seq, PublishMessageStatus.EXPIRED_SESSION);
            if (session.ClientPurpose != ClientPurposeTypes.PUBLISH) return PackageResponseBuilder.PublishMessage(request.Seq, PublishMessageStatus.BAD_SESSION_USAGE);
            var allQueues = await GetAllStateMachines<QueueStateMachine>(QUEUE_PARTITION_KEY, cancellationToken);
            // TODO : Redirect the message to the correct queue.
            var filteredQueues = allQueues;
            var publishedQueueNames = new ConcurrentBag<string>();
            var id = Guid.NewGuid().ToString();
            await Parallel.ForEachAsync(filteredQueues, new ParallelOptions
            {
                MaxDegreeOfParallelism = _eventMeshOptions.MaxNbThreads
            }, async (q, t) =>
            {
                var addMessageCommand = new AddQueueMessageCommand { Data = request.CloudEvent, Topic = request.Topic };
                var result = await Send(q.QueueName, id, addMessageCommand, cancellationToken);
                if (result.Success) publishedQueueNames.Add(q.QueueName);
            });

            return PackageResponseBuilder.PublishMessage(request.Seq, publishedQueueNames.ToArray());
        }
    }
}
