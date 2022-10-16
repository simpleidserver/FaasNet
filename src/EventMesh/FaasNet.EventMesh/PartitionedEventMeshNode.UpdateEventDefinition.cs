using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(UpdateEventDefinitionRequest updateEventDefinition, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = updateEventDefinition.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.UpdateEventDefinition(updateEventDefinition.Seq, UpdateEventDefinitionStatus.UNKNOWN_VPN);
            var eventDef = await Query<GetEventDefinitionQueryResult>(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new GetEventDefinitionQuery { Id = updateEventDefinition.Id, Vpn = updateEventDefinition.Vpn }, cancellationToken);
            if (!eventDef.Success) return PackageResponseBuilder.UpdateEventDefinition(updateEventDefinition.Seq, UpdateEventDefinitionStatus.NOT_FOUND);
            var result = await Send(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new UpdateEventDefinitionCommand { Id = updateEventDefinition.Id, JsonSchema = updateEventDefinition.JsonSchema, Vpn = updateEventDefinition.Vpn }, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.UpdateEventDefinition(updateEventDefinition.Seq, UpdateEventDefinitionStatus.NO_LEADER);
            return PackageResponseBuilder.UpdateEventDefinition(updateEventDefinition.Seq, UpdateEventDefinitionStatus.OK);
        }
    }
}
