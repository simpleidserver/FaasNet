using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.ApplicationDomain;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddLinkApplicationDomainRequest addLinkApplicationDomain, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = addLinkApplicationDomain.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.AddLinkApplicationDomain(addLinkApplicationDomain.Seq, AddLinkApplicationDomainStatus.UNKNOWN_VPN);
            var evtDef = await Query<GetApplicationDomainQueryResult>(PartitionNames.APPLICATION_DOMAIN, new  GetApplicationDomainQuery { Name = addLinkApplicationDomain.Name, Vpn = addLinkApplicationDomain.Vpn }, cancellationToken);
            if (!evtDef.Success) return PackageResponseBuilder.AddLinkApplicationDomain(addLinkApplicationDomain.Seq, AddLinkApplicationDomainStatus.NOT_FOUND);
            var result = await Send(PartitionNames.APPLICATION_DOMAIN, new AddApplicationDomainLinkCommand { Name = addLinkApplicationDomain.Name, Vpn = addLinkApplicationDomain.Vpn, EventId = addLinkApplicationDomain.EventId, Source = addLinkApplicationDomain.Source, Target = addLinkApplicationDomain.Target }, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.AddLinkApplicationDomain(addLinkApplicationDomain.Seq, AddLinkApplicationDomainStatus.NOLEADER);
            return PackageResponseBuilder.AddLinkApplicationDomain(addLinkApplicationDomain.Seq, AddLinkApplicationDomainStatus.OK);
        }
    }
}
