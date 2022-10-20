using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.EventMesh.Client.StateMachines.Vpn;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(AddEventDefinitionRequest addEventDefinition, CancellationToken cancellationToken)
        {
            var vpn = await Query<GetVpnQueryResult>(PartitionNames.VPN_PARTITION_KEY, new GetVpnQuery { Id = addEventDefinition.Vpn }, cancellationToken);
            if (!vpn.Success) return PackageResponseBuilder.AddEventDefinition(addEventDefinition.Seq, addEventDefinition.Id, AddEventDefinitionStatus.UNKNOWN_VPN);
            var eventDef = await Query<GetEventDefinitionQueryResult>(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new GetEventDefinitionQuery { Id = addEventDefinition.Id, Vpn = addEventDefinition.Vpn }, cancellationToken);
            if (eventDef.Success) return PackageResponseBuilder.AddEventDefinition(addEventDefinition.Seq, addEventDefinition.Id, AddEventDefinitionStatus.EXISTING_EVENTDEFINITION);
            var addEventDefCmd = new AddEventDefinitionCommand { Id = addEventDefinition.Id, JsonSchema = addEventDefinition.JsonSchema, Vpn = addEventDefinition.Vpn, Description = addEventDefinition.Description };
            var result = await Send(PartitionNames.EVENTDEFINITION_PARTITION_KEY, addEventDefCmd, cancellationToken);
            if (!result.Success) return PackageResponseBuilder.AddEventDefinition(addEventDefinition.Seq, addEventDefinition.Id, AddEventDefinitionStatus.NOLEADER);
            return PackageResponseBuilder.AddEventDefinition(addEventDefinition.Seq, addEventDefinition.Id, result.Term, result.MatchIndex, result.LastIndex);
        }
    }
}
