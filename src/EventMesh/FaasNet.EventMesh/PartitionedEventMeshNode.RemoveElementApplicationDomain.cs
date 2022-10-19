using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(RemoveElementApplicationDomainRequest removeEltApplicationDomain, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = removeEltApplicationDomain.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.RemoveApplicationDomainElement(removeEltApplicationDomain.Seq, RemoveElementApplicationDomainStatus.UNKNOWN_VPN);
            var removeApplicationDomainEltCmd = new RemoveApplicationDomainElementCommand { ElementId = removeEltApplicationDomain.ElementId, Name = removeEltApplicationDomain.Name, Vpn = removeEltApplicationDomain.Vpn };
            var result = await Send(PartitionNames.APPLICATION_DOMAIN, removeApplicationDomainEltCmd, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.RemoveApplicationDomainElement(removeEltApplicationDomain.Seq, RemoveElementApplicationDomainStatus.NOLEADER);
            return PackageResponseBuilder.RemoveApplicationDomainElement(removeEltApplicationDomain.Seq, RemoveElementApplicationDomainStatus.OK);
        }
    }
}
