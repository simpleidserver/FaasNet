using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetAllApplicationDomainsRequest getAllApplicationDomainsRequest, CancellationToken cancellationToken)
        {
            var res = await Query<GenericSearchQueryResult<ApplicationDomainQueryResult>>(PartitionNames.APPLICATION_DOMAIN, new GetAllApplicationDomainsQuery { Filter = getAllApplicationDomainsRequest.Filter }, cancellationToken);
            return PackageResponseBuilder.GetAllApplicationDomains(getAllApplicationDomainsRequest.Seq, res);
        }
    }
}
