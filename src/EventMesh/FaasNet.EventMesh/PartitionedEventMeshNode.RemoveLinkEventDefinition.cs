using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.Client;
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
            var vpn = await Query<GetVpnQueryResult>(VPN_PARTITION_KEY, new GetVpnQuery { Id = removeEventDefRequest.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.RemoveLinkEventDefinition(removeEventDefRequest.Seq, RemoveEventDefinitionStatus.UNKNOWN_VPN);
            var evtDef = await Query<GetEventDefinitionQueryResult>(EVENTDEFINITION_PARTITION_KEY, new GetEventDefinitionQuery { Id = removeEventDefRequest.Id, Vpn = removeEventDefRequest.Vpn }, cancellationToken);
            if (!evtDef.Success) return PackageResponseBuilder.RemoveLinkEventDefinition(removeEventDefRequest.Seq, RemoveEventDefinitionStatus.NOT_FOUND);
            await Send(CLIENT_PARTITION_KEY, new RemoveTargetCommand { ClientId = removeEventDefRequest.Source, Vpn = removeEventDefRequest.Vpn, EventId = removeEventDefRequest.Id, Target = removeEventDefRequest.Target  }, cancellationToken);
            var result = await Send(EVENTDEFINITION_PARTITION_KEY, new RemoveLinkEventDefinitionCommand { Id = removeEventDefRequest.Id, Vpn = removeEventDefRequest.Vpn, Source = removeEventDefRequest.Source, Target = removeEventDefRequest.Target }, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.RemoveLinkEventDefinition(removeEventDefRequest.Seq, RemoveEventDefinitionStatus.NOLEADER);
            return PackageResponseBuilder.RemoveLinkEventDefinition(removeEventDefRequest.Seq, RemoveEventDefinitionStatus.OK);
        }
    }
}
