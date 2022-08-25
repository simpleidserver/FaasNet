using FaasNet.EventMesh;
using FaasNet.Peer;
using FaasNet.RaftConsensus.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace FaasNet.Partition
{
    public static class PartitionedNodeHostFactoryExtensions
    {
        public static PartitionedNodeHostFactory UseEventMesh(this PartitionedNodeHostFactory partitionedNodeHostFactory)
        {
            RemovePartitionPeerFactory(partitionedNodeHostFactory);
            RemovePeerHost(partitionedNodeHostFactory);
            partitionedNodeHostFactory.Services.AddTransient<IPartitionPeerFactory, RaftConsensusPartitionPeerFactory>();
            partitionedNodeHostFactory.Services.AddScoped<IPeerHost, PartitionedEventMeshNode>();
            return partitionedNodeHostFactory;
        }

        private static void RemovePartitionPeerFactory(PartitionedNodeHostFactory partitionedNodeHostFactory) => RemoveDependency<IPartitionPeerFactory>(partitionedNodeHostFactory);
        private static void RemovePeerHost(PartitionedNodeHostFactory partitionedNodeHostFactory) => RemoveDependency<IPeerHost>(partitionedNodeHostFactory);

        private static void RemoveDependency<T>(PartitionedNodeHostFactory partitionedNodeHostFactory)
        {
            var registeredType = partitionedNodeHostFactory.Services.FirstOrDefault(s => s.ServiceType == typeof(T));
            if (registeredType != null) partitionedNodeHostFactory.Services.Remove(registeredType);
        }
    }
}
