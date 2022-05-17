using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IQueueStore
    {
        Task<string> Dequeue(string queueName, CancellationToken cancellationToken);
        Task Add(string queueName, string message, CancellationToken cancellationToken);
    }

    public class QueueStore : IQueueStore
    {
        private readonly INodeStateStore _nodeStateStore;

        public QueueStore(INodeStateStore nodeStateStore)
        {
            _nodeStateStore = nodeStateStore;
        }

        public async Task<string> Dequeue(string queueName, CancellationToken cancellationToken)
        {
            var consumedQueueMessage = string.Format(StandardEntityTypes.ConsumedQueueMessage, queueName);
            var lastEntityType = await _nodeStateStore.GetLastEntityType(consumedQueueMessage, cancellationToken);
            var version = lastEntityType == null ? 0 : lastEntityType.EntityVersion;
            var entityType = await _nodeStateStore.GetSpecificEntityType(string.Format(StandardEntityTypes.Queue, queueName), version, cancellationToken);
            if (entityType == null) return null;
            var nodeState = new NodeState { EntityId = Guid.NewGuid().ToString(), EntityType = string.Format(StandardEntityTypes.ConsumedQueueMessage, queueName), EntityVersion = version + 1, Value = queueName };
            _nodeStateStore.Add(nodeState);
            return entityType.Value;
        }

        public async Task Add(string queueName, string message, CancellationToken cancellationToken)
        {
            var entityType = string.Format(StandardEntityTypes.Queue, queueName);
            var nodeState = new NodeState { EntityId = Guid.NewGuid().ToString(), EntityType = string.Format(StandardEntityTypes.Queue, queueName), EntityVersion = 0, Value = message };
            var existingValue = await _nodeStateStore.GetLastEntityType(entityType, cancellationToken);
            if (existingValue != null) nodeState.EntityVersion = existingValue.EntityVersion + 1;
            _nodeStateStore.Add(nodeState);
        }
    }
}
