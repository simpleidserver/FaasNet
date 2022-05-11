using FaasNet.EventMesh.Runtime.Models;
using FaasNet.RaftConsensus.Core.Stores;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IClientSessionStore
    {
        Task<ClientSession> Get(string sessionId, CancellationToken cancellationToken);
        Task Add(ClientSession session, CancellationToken cancellationToken);
    }

    public class ClientSessionStore : IClientSessionStore
    {
        private readonly INodeStateStore _nodeStateStore;

        public ClientSessionStore(INodeStateStore nodeStateStore)
        {
            _nodeStateStore = nodeStateStore;
        }

        public async Task<ClientSession> Get(string sessionId, CancellationToken cancellationToken)
        {
            var nodeState = await _nodeStateStore.GetLastEntityId(sessionId, cancellationToken);
            if (nodeState == null) return null;
            return JsonSerializer.Deserialize<ClientSession>(nodeState.Value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task Add(ClientSession session, CancellationToken cancellationToken)
        {
            var lastEntityId = await _nodeStateStore.GetLastEntityId(session.Id, cancellationToken);
            var nodeState = session.ToNodeState();
            if (lastEntityId != null) nodeState.EntityVersion = lastEntityId.EntityVersion + 1;
            _nodeStateStore.Add(nodeState);
        }
    }
}
