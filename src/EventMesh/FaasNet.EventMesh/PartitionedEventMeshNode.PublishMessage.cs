using FaasNet.EventMesh.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public Task<BaseEventMeshPackage> Handle(PublishMessageRequest request, CancellationToken cancellationToken)
        {
            // Check session exists.

            return Task.FromResult((BaseEventMeshPackage)null);
        }
    }
}
