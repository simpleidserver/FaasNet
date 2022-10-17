using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(RemoveLinkApplicationDomain removeEventDefRequest, CancellationToken cancellationToken)
        {
            // TODO : CONTINUE
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = removeEventDefRequest.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.RemoveLinkApplicationDomain(removeEventDefRequest.Seq, RemoveLinkApplicationDomainStatus.UNKNOWN_VPN);
            var evtDef = await Query<GetEventDefinitionQueryResult>(PartitionNames.APPLICATION_DOMAIN, new  { Id = removeEventDefRequest.Id, Vpn = removeEventDefRequest.Vpn }, cancellationToken);
            if (!evtDef.Success) return PackageResponseBuilder.RemoveLinkApplicationDomain(removeEventDefRequest.Seq, RemoveLinkApplicationDomainStatus.NOT_FOUND);
            var result = await Send(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new RemoveLinkEventDefinitionCommand { Id = removeEventDefRequest.Id, Vpn = removeEventDefRequest.Vpn, Source = removeEventDefRequest.Source, Target = removeEventDefRequest.Target }, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.RemoveLinkApplicationDomain(removeEventDefRequest.Seq, RemoveLinkApplicationDomainStatus.NOLEADER);
            return PackageResponseBuilder.RemoveLinkApplicationDomain(removeEventDefRequest.Seq, RemoveLinkApplicationDomainStatus.OK);
        }
    }
}
