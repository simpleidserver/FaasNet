using FaasNet.Common.Extensions;
using FaasNet.RaftConsensus.Core.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface INodeStateStore
    {
        void Add(NodeState nodeState);
        void Update(NodeState nodeState);
        Task<int> SaveChanges(CancellationToken cancellationToken);
        Task<IEnumerable<NodeState>> GetAllEntityTypes(CancellationToken cancellationToken);
        Task<IEnumerable<NodeState>> GetAllLastEntityTypes(CancellationToken cancellationToken);
        Task<IEnumerable<NodeState>> GetAllLastEntityTypes(string entityType, CancellationToken cancellationToken);
        Task<IEnumerable<NodeState>> GetAllSpecificEntityTypes(List<(string EntityType, int EntityVersion)> parameter, CancellationToken cancellationToken);
        Task<NodeState> GetLastEntityType(string entityType, CancellationToken cancellationToken);
    }

    public class InMemoryNodeStateStore : INodeStateStore
    {
        private readonly ConcurrentBag<NodeState> _nodeStates;

        public InMemoryNodeStateStore() 
        {
            _nodeStates = new ConcurrentBag<NodeState>();
        }

        public InMemoryNodeStateStore(ConcurrentBag<NodeState> nodeStates)
        {
            _nodeStates = nodeStates;
        }

        public void Add(NodeState nodeState)
        {
            _nodeStates.Add(nodeState);
        }

        public void Update(NodeState nodeState)
        {
            _nodeStates.Remove(nodeState);
        }

        public Task<IEnumerable<NodeState>> GetAllEntityTypes(CancellationToken cancellationToken)
        {
            IEnumerable<NodeState> result = _nodeStates.OrderByDescending(ns => ns.EntityVersion);
            return Task.FromResult(result);
        }

        public Task<IEnumerable<NodeState>> GetAllLastEntityTypes(CancellationToken cancellationToken)
        {
            IEnumerable<NodeState> result = _nodeStates.OrderByDescending(ns => ns.EntityVersion).GroupBy(ns => ns.EntityType).Select(ns => ns.First());
            return Task.FromResult(result);
        }

        public Task<IEnumerable<NodeState>> GetAllLastEntityTypes(string entityType, CancellationToken cancellationToken)
        {
            IEnumerable<NodeState> result = _nodeStates.Where(ns => ns.EntityType == entityType).OrderByDescending(ns => ns.EntityVersion).GroupBy(ns => ns.EntityId).Select(ns => ns.First());
            return Task.FromResult(result);
        }

        public Task<IEnumerable<NodeState>> GetAllSpecificEntityTypes(List<(string EntityType, int EntityVersion)> parameter, CancellationToken cancellationToken)
        {
            IEnumerable<NodeState> result = _nodeStates.Where(ns => parameter.Any(p => p.EntityType == ns.EntityType && p.EntityVersion == ns.EntityVersion));
            return Task.FromResult(result);
        }

        public Task<NodeState> GetLastEntityType(string entityType, CancellationToken cancellationToken)
        {
            return Task.FromResult(_nodeStates.OrderByDescending(ns => ns.EntityVersion).FirstOrDefault(ns => ns.EntityType == entityType));
        }

        public Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return Task.FromResult(1);
        }
    }
}
