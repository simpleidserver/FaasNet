using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(FindClientsByNameRequest request, CancellationToken cancellationToken)
        {
            var res = await Query<FindClientsByNameQueryResult>(CLIENT_PARTITION_KEY, new FindClientsByNameQuery { Name = request.Name }, cancellationToken);
            return PackageResponseBuilder.FindClientsByName(request.Seq, res.Content);
        }
    }
}
