using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Queue;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(SearchQueuesRequest request, CancellationToken cancellationToken)
        {
            var res = await Query<GenericSearchQueryResult<QueueQueryResult>>(QUEUE_PARTITION_KEY, new FindQueuesQuery
            {
                Filter = request.Filter
            }, cancellationToken);
            return PackageResponseBuilder.SearchQueues(request.Seq, res);
        }
    }
}
