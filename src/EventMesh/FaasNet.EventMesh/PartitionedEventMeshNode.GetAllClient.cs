using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetAllClientRequest getAllClientRequest, CancellationToken cancellationToken)
        {
            var clientCollection = await GetStateMachine<ClientCollection>(CLIENT_PARTITION_KEY, cancellationToken);
            return PackageResponseBuilder.GetAllClient(getAllClientRequest.Seq, clientCollection.Values.Select(v => new ClientResult
            {
                Id = v.Id,
                Vpn = v.Vpn,
                Purposes = v.Purposes
            }).ToList());
        }
    }
}
