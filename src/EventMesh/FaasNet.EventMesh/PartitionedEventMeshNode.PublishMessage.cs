using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(PublishMessageRequest request, CancellationToken cancellationToken)
        {
            var session = await GetStateMachine<SessionStateMachine>(SESSION_PARTITION_KEY, request.SessionId, cancellationToken);
            if (session == null) return PackageResponseBuilder.PublishMessage(request.Seq, PublishMessageStatus.UNKNOWN_SESSION);
            if (!session.IsValid) return PackageResponseBuilder.PublishMessage(request.Seq, PublishMessageStatus.EXPIRED_SESSION);
            if (session.ClientPurpose != ClientPurposeTypes.PUBLISH) return PackageResponseBuilder.PublishMessage(request.Seq, PublishMessageStatus.BAD_SESSION_USAGE);
            var allQueues = await GetAllStateMachines<QueueStateMachine>(QUEUE_PARTITION_KEY, cancellationToken);
            var filteredQueues = allQueues;
            var id = Guid.NewGuid().ToString();
            await Parallel.ForEachAsync(filteredQueues, new ParallelOptions
            {
                MaxDegreeOfParallelism = _eventMeshOptions.MaxNbThreads
            }, async (q, t) =>
            {
                var addMessageCommand = new AddQueueMessageCommand { Data = request.CloudEvent, Topic = request.Topic };
                await Send(q.QueueName, id, addMessageCommand, cancellationToken);
            });

            return PackageResponseBuilder.PublishMessage(request.Seq);
        }
    }
}
