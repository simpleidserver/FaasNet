using FaasNet.RaftConsensus.Core.Stores;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IClientStore
    {
        Task Add(Models.Client client, CancellationToken cancellationToken);
        Task<Models.Client> Get(string vpn, string clientId, CancellationToken cancellationToken);
    }

    public class ClientStore : IClientStore
    {
        private readonly INodeStateStore _nodeStateStore;

        public ClientStore(INodeStateStore nodeStateStore)
        {
            _nodeStateStore = nodeStateStore;
        }

        public async Task Add(Models.Client client, CancellationToken cancellationToken)
        {
            var lastEntityId = await _nodeStateStore.GetLastEntityId(client.Id, cancellationToken);
            var nodeState = client.ToNodeState();
            if (lastEntityId != null) nodeState.EntityVersion = lastEntityId.EntityVersion + 1;
            _nodeStateStore.Add(nodeState);
        }

        public async Task<Models.Client> Get(string vpn, string clientId, CancellationToken cancellationToken)
        {
            var nodeState = await _nodeStateStore.GetLastEntityId(Models.Client.BuildId(vpn, clientId), cancellationToken);
            if (nodeState == null) return null;
            return JsonSerializer.Deserialize<Models.Client>(nodeState.Value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
