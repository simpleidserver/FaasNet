using FaasNet.EventMesh.Runtime.Models;
using FaasNet.RaftConsensus.Core.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IBridgeStore
    {
        Task<IEnumerable<BridgeServer>> GetAll(CancellationToken cancellationToken);
        Task Add(BridgeServer vpn, CancellationToken cancellationToken);
    }

    public class BridgeServerStore : IBridgeStore
    {
        private readonly INodeStateStore _nodeStateStore;

        public BridgeServerStore(INodeStateStore nodeStateStore)
        {
            _nodeStateStore = nodeStateStore;
        }

        public async Task<IEnumerable<BridgeServer>> GetAll(CancellationToken cancellationToken)
        {
            var lastEntityTypes = await _nodeStateStore.GetAllLastEntityTypes(StandardEntityTypes.BridgeServer, cancellationToken);
            lastEntityTypes = lastEntityTypes.OrderBy(e => e.EntityVersion);
            var result = lastEntityTypes.Select(et => JsonSerializer.Deserialize<BridgeServer>(et.Value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }));
            return result;
        }

        public async Task Add(BridgeServer vpn, CancellationToken cancellationToken)
        {
            var lastEntityType = await _nodeStateStore.GetLastEntityType(StandardEntityTypes.BridgeServer, cancellationToken);
            var nodeState = vpn.ToNodeState();
            if (lastEntityType != null) nodeState.EntityVersion = lastEntityType.EntityVersion + 1;
            _nodeStateStore.Add(nodeState);
        }
    }
}
