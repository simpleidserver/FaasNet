using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddLinkApplicationDomainRequest removeLinkApplicationDomain, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = removeLinkApplicationDomain.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.RemoveLinkApplicationDomain(removeLinkApplicationDomain.Seq, RemoveLinkApplicationDomainStatus.UNKNOWN_VPN);
            var evtDef = await Query<GetEventDefinitionQueryResult>(PartitionNames.APPLICATION_DOMAIN, new  GetApplicationDomainQuery { Name = removeLinkApplicationDomain.Name, Vpn = removeLinkApplicationDomain.Vpn }, cancellationToken);
            if (!evtDef.Success) return PackageResponseBuilder.RemoveLinkApplicationDomain(removeLinkApplicationDomain.Seq, RemoveLinkApplicationDomainStatus.NOT_FOUND);
            var result = await Send(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new RemoveApplicationDomainLinkCommand { Name = removeLinkApplicationDomain.Name, Vpn = removeLinkApplicationDomain.Vpn, EventId = removeLinkApplicationDomain.EventId }, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.RemoveLinkApplicationDomain(removeLinkApplicationDomain.Seq, RemoveLinkApplicationDomainStatus.NOLEADER);
            return PackageResponseBuilder.RemoveLinkApplicationDomain(removeLinkApplicationDomain.Seq, RemoveLinkApplicationDomainStatus.OK);
        }
    }
}
