using FaasNet.RaftConsensus.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface IClusterStore
    {
        Task SelfRegister(ClusterNode node, CancellationToken cancellationToken);
        Task<IEnumerable<ClusterNode>> GetAllNodes(CancellationToken cancellationToken);
    }

    public class InMemoryClusterStore : IClusterStore
    {
        private static object _obj = new object();
        private readonly ICollection<ClusterNode> _clusterNodes;

        public InMemoryClusterStore()
        {
            _clusterNodes = new List<ClusterNode>();
        }

        public Task SelfRegister(ClusterNode node, CancellationToken cancellationToken)
        {
            lock(_obj)
            {
                _clusterNodes.Add(node);
                return Task.CompletedTask;
            }
        }

        public Task<IEnumerable<ClusterNode>> GetAllNodes(CancellationToken cancellationToken)
        {
            lock(_obj)
            {
                IEnumerable<ClusterNode> result = _clusterNodes;
                return Task.FromResult(result);
            }
        }
    }
}
