using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetApplicationDomainRequest getApplicationDomainRequest, CancellationToken cancellationToken)
        {
            var result = await Query<GetApplicationDomainQueryResult>(PartitionNames.APPLICATION_DOMAIN, new GetApplicationDomainQuery { Name = getApplicationDomainRequest.Name, Vpn = getApplicationDomainRequest.Vpn }, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.GetApplicationDomain(getApplicationDomainRequest.Seq);
            return PackageResponseBuilder.GetApplicationDomain(getApplicationDomainRequest.Seq, result.Content);
        }
    }
}