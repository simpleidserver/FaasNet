using FaasNet.RaftConsensus.Core;
using Microsoft.Extensions.DependencyInjection;

namespace FaasNet.Partition
{
    public static class PartitionedNodeHostFactoryExtensions
    {
        public static PartitionedNodeHostFactory UseRaftConsensusPeer(this PartitionedNodeHostFactory partitionedNodeHostFactory)
        {
            partitionedNodeHostFactory.Services.AddTransient<IPartitionPeerFactory, RaftConsensusPartitionPeerFactory>();
            return partitionedNodeHostFactory;
        }
    }
}
