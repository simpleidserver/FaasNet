using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IQueueStore
    {
        Task<string> Get(string queueName, int offset, CancellationToken cancellationToken);
        Task Add(string queueName, string message, CancellationToken cancellationToken);
    }

    public class QueueStore : IQueueStore
    {
        private readonly INodeStateStore _nodeStateStore;

        public QueueStore(INodeStateStore nodeStateStore)
        {
            _nodeStateStore = nodeStateStore;
        }

        public async Task<string> Get(string queueName, int offset, CancellationToken cancellationToken)
        {
            var message = await _nodeStateStore.GetSpecificEntityType(string.Format(StandardEntityTypes.Queue, queueName), offset, cancellationToken);
            return message?.Value;
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
