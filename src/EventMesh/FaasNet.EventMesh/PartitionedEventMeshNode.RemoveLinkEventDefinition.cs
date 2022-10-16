using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(RemoveLinkEventDefinitionRequest removeEventDefRequest, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = removeEventDefRequest.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.RemoveLinkEventDefinition(removeEventDefRequest.Seq, RemoveEventDefinitionStatus.UNKNOWN_VPN);
            var evtDef = await Query<GetEventDefinitionQueryResult>(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new GetEventDefinitionQuery { Id = removeEventDefRequest.Id, Vpn = removeEventDefRequest.Vpn }, cancellationToken);
            if (!evtDef.Success) return PackageResponseBuilder.RemoveLinkEventDefinition(removeEventDefRequest.Seq, RemoveEventDefinitionStatus.NOT_FOUND);
            var result = await Send(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new RemoveLinkEventDefinitionCommand { Id = removeEventDefRequest.Id, Vpn = removeEventDefRequest.Vpn, Source = removeEventDefRequest.Source, Target = removeEventDefRequest.Target }, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.RemoveLinkEventDefinition(removeEventDefRequest.Seq, RemoveEventDefinitionStatus.NOLEADER);
            return PackageResponseBuilder.RemoveLinkEventDefinition(removeEventDefRequest.Seq, RemoveEventDefinitionStatus.OK);
        }
    }
}
