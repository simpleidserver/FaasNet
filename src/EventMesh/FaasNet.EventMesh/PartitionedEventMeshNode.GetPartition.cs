using FaasNet.EventMesh.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetPartitionRequest getPartitionRequest, CancellationToken cancellationToken)
        {
            if (!await PartitionCluster.Contains(PartitionNames.CLIENT_PARTITION_KEY, cancellationToken)) return PackageResponseBuilder.GetPartition(getPartitionRequest.Seq, GetPartitionStatus.NOPARTITION);
            var peerState = await GetPeerState(PartitionNames.CLIENT_PARTITION_KEY, cancellationToken);
            return PackageResponseBuilder.GetPartition(getPartitionRequest.Seq, peerState);
        }
    }
}
