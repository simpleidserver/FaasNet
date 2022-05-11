using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface IPeerStore
    {
        Task<IEnumerable<Peer>> GetAll(CancellationToken cancellationToken);
        Task Add(Peer peer, CancellationToken cancellationToken);
        Task<Peer> Get(string termId, CancellationToken cancellationToken);
    }

    public class InMemoryPeerStore : IPeerStore
    {
        private readonly INodeStateStore _nodeStateStore;

        public InMemoryPeerStore(INodeStateStore nodeStateStore)
        {
            _nodeStateStore = nodeStateStore;
        }

        public async Task<IEnumerable<Peer>> GetAll(CancellationToken cancellationToken)
        {
            var lastEntityTypes = await _nodeStateStore.GetAllLastEntityTypes(StandardEntityTypes.Peer, cancellationToken);
            lastEntityTypes = lastEntityTypes.OrderBy(e => e.EntityVersion);
            var result = lastEntityTypes.Select(et => JsonSerializer.Deserialize<Peer>(et.Value, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }));
            return result;
        }

        public async Task Add(Peer peer, CancellationToken cancellationToken)
        {
            var lastEntityType = await _nodeStateStore.GetLastEntityType(StandardEntityTypes.Peer, cancellationToken);
            var nodeState = peer.ToNodeState();
            if (lastEntityType != null) nodeState.EntityVersion = lastEntityType.EntityVersion + 1;
            _nodeStateStore.Add(nodeState);
        }

        public async Task<Peer> Get(string termId, CancellationToken cancellationToken)
        {
            var allNodes = await GetAll(cancellationToken);
            return allNodes.FirstOrDefault(n => n.TermId == termId);
        }
    }
}
