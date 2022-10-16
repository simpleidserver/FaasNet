using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Queue;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(FindQueuesByNameRequest request, CancellationToken cancellationToken)
        {
            var res = await Query<FindQueuesByNameQueryResult>(PartitionNames.QUEUE_PARTITION_KEY, new FindQueuesByNameQuery { Name = request.Name }, cancellationToken);
            return PackageResponseBuilder.FindQueuesByName(request.Seq, res.Content);
        }
    }
}
