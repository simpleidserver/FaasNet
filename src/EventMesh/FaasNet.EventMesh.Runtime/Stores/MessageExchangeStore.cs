using FaasNet.EventMesh.Runtime.Models;
using FaasNet.RaftConsensus.Core.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IMessageExchangeStore
    {
        Task<IEnumerable<MessageExchange>> GetAll(CancellationToken cancellationToken);
        Task<MessageExchange> Get(string topicFilter, CancellationToken cancellationToken);
        Task Add(MessageExchange messageExchange, CancellationToken cancellationToken);
    }

    public class MessageExchangeStore : IMessageExchangeStore
    {
        private readonly INodeStateStore _nodeStateStore;

        public MessageExchangeStore(INodeStateStore nodeStateStore)
        {
            _nodeStateStore = nodeStateStore;
        }

        public async Task<IEnumerable<MessageExchange>> GetAll(CancellationToken cancellationToken)
        {
            var result = await _nodeStateStore.GetAllLastEntityTypes(cancellationToken);
            return result.Select(n => JsonSerializer.Deserialize<MessageExchange>(n.Value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
        }

        public async Task Add(MessageExchange messageExchange, CancellationToken cancellationToken)
        {
            var nodeState = messageExchange.ToNodeState();
            var lastEntityId = await _nodeStateStore.GetLastEntityId(nodeState.EntityId, cancellationToken);
            if (lastEntityId != null) nodeState.EntityVersion = lastEntityId.EntityVersion + 1;
            _nodeStateStore.Add(nodeState);
        }

        public async Task<MessageExchange> Get(string topicFilter, CancellationToken cancellationToken)
        {
            var lastEntityId = await _nodeStateStore.GetLastEntityId(topicFilter, cancellationToken);
            if (lastEntityId == null) return null;
            return JsonSerializer.Deserialize<MessageExchange>(lastEntityId.Value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
