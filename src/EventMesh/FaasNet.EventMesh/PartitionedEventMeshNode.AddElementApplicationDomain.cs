using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddElementApplicationDomainRequest addElemenApplicationDomain, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = addElemenApplicationDomain.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.AddApplicationDomainElement(addElemenApplicationDomain.Seq, AddElementApplicationDomainStatus.UNKNOWN_VPN);
            var applicationDomain = await Query<GetApplicationDomainQueryResult>(PartitionNames.APPLICATION_DOMAIN, new  GetApplicationDomainQuery { Name = addElemenApplicationDomain.Name, Vpn = addElemenApplicationDomain.Vpn }, cancellationToken);
            if (!applicationDomain.Success) return PackageResponseBuilder.AddApplicationDomainElement(addElemenApplicationDomain.Seq, AddElementApplicationDomainStatus.NOT_FOUND);
            var result = await Send(PartitionNames.APPLICATION_DOMAIN, new AddApplicationDomainElementCommand { Name = addElemenApplicationDomain.Name, Vpn = addElemenApplicationDomain.Vpn, CoordinateX = addElemenApplicationDomain.CoordinateX, CoordinateY = addElemenApplicationDomain.CoordinateY, ElementId = addElemenApplicationDomain.ElementId }, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.AddApplicationDomainElement(addElemenApplicationDomain.Seq, AddElementApplicationDomainStatus.NOLEADER);
            return PackageResponseBuilder.AddApplicationDomainElement(addElemenApplicationDomain.Seq, AddElementApplicationDomainStatus.OK);
        }
    }
}
