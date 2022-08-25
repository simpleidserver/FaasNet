using FaasNet.EventMesh.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public Task<BaseEventMeshPackage> Handle(PingRequest request, CancellationToken cancellationToken)
        {
            var result = PackageResponseBuilder.HeartBeat(request.Seq);
            return Task.FromResult(result);
        }
    }
}
