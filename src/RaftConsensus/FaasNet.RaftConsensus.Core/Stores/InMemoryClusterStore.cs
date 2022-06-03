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

    public class InMemoryClusterPool
    {
        private static object _obj = new object();
        private static ICollection<ClusterNode> _clusterNodes = new List<ClusterNode>();

        public static ICollection<ClusterNode> ClusterNodes
        {
            get
            {
                lock(_obj)
                {
                    if (_clusterNodes == null)
                    {
                        _clusterNodes = new List<ClusterNode>();
                    }

                    return _clusterNodes;
                }
            }
        }

        public static void Add(ClusterNode clusterNode)
        {
            lock(_obj)
            {
                ClusterNodes.Add(clusterNode);
            }
        }
    }

    public class InMemoryClusterStore : IClusterStore
    {
        public Task SelfRegister(ClusterNode node, CancellationToken cancellationToken)
        {
            InMemoryClusterPool.Add(node);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ClusterNode>> GetAllNodes(CancellationToken cancellationToken)
        {
            IEnumerable<ClusterNode> result = InMemoryClusterPool.ClusterNodes;
            return Task.FromResult(result);
        }
    }
}
