using FaasNet.EventMesh.Runtime.Models;
using FaasNet.RaftConsensus.Core.Stores;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public interface IVpnStore
    {
        Task<IEnumerable<Vpn>> GetAll(CancellationToken cancellationToken);
        Task<Vpn> Get(string name, CancellationToken cancellationToken);
        Task Add(Vpn vpn, CancellationToken cancellationToken);
    }

    public class VpnStore : IVpnStore
    {
        private readonly INodeStateStore _nodeStateStore;

        public VpnStore(INodeStateStore nodeStateStore)
        {
            _nodeStateStore = nodeStateStore;
        }

        public async Task<IEnumerable<Vpn>> GetAll(CancellationToken cancellationToken)
        {
            var lastEntityTypes = await _nodeStateStore.GetAllLastEntityTypes(StandardEntityTypes.Vpn, cancellationToken);
            lastEntityTypes = lastEntityTypes.OrderBy(e => e.EntityVersion);
            var result = lastEntityTypes.Select(et => JsonSerializer.Deserialize<Vpn>(et.Value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }));
            return result;
        }

        public async Task<Vpn> Get(string name, CancellationToken cancellationToken)
        {
            var allNodes = await GetAll(cancellationToken);
            return allNodes.FirstOrDefault(n => n.Name == name);
        }

        public async Task Add(Vpn vpn, CancellationToken cancellationToken)
        {
            var lastEntityType = await _nodeStateStore.GetLastEntityType(StandardEntityTypes.Vpn, cancellationToken);
            var nodeState = vpn.ToNodeState();
            if (lastEntityType != null) nodeState.EntityVersion = lastEntityType.EntityVersion + 1;
            _nodeStateStore.Add(nodeState);
        }
    }
}
