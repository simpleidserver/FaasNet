using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(UpdateApplicationDomainCoordinatesRequest request, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = request.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.UpdateApplicationDomainCoordinates(request.Seq, UpdateApplicationDomainCoordinatesStatus.UNKNOWN_VPN);
            var updateApplicationDomainCoordinatesCmd = new UpdateApplicationDomainCoordinatesCommand
            {
                Name = request.Name,
                Vpn = request.Vpn,
                Elements = request.Coordinates.Select(c => new UpdateApplicationDomainElement
                {
                    CoordinateX = c.CoordinateX,
                    CoordinateY = c.CoordinateY,
                    ElementId = c.ElementId
                }).ToList()
            };
            var result = await Send(PartitionNames.APPLICATION_DOMAIN, updateApplicationDomainCoordinatesCmd, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.UpdateApplicationDomainCoordinates(request.Seq, UpdateApplicationDomainCoordinatesStatus.NOLEADER);
            return PackageResponseBuilder.UpdateApplicationDomainCoordinates(request.Seq, UpdateApplicationDomainCoordinatesStatus.OK);
        }
    }
}
