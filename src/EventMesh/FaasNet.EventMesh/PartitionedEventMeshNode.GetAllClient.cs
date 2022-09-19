using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetAllClientRequest getAllClientRequest, CancellationToken cancellationToken)
        {
            var res = await Query<GetAllClientsQueryResult>(CLIENT_PARTITION_KEY, new GetAllClientsQuery(), cancellationToken);
            return PackageResponseBuilder.GetAllClient(getAllClientRequest.Seq, res.Clients.Select(v => new ClientResult
            {
                Id = v.Id,
                Vpn = v.Vpn,
                Purposes = v.Purposes,
                CreateDateTime = v.CreateDateTime
            }).ToList());
        }
    }
}
