using FaasNet.EventMesh.Client.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(HelloRequest helloRequest, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
