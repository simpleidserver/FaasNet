using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetEventDefinitionRequest getEventDefinition, CancellationToken cancellationToken)
        {
            var res = await Query<GetEventDefinitionQueryResult>(EVENTDEFINITION_PARTITION_KEY, new GetEventDefinitionQuery { Id = getEventDefinition.Id, Vpn = getEventDefinition.Vpn }, cancellationToken);
            if (res == null) return PackageResponseBuilder.GetEventDefinition(getEventDefinition.Seq, GetEventDefinitionStatus.NOT_FOUND);
            return PackageResponseBuilder.GetEventDefinition(getEventDefinition.Seq, res.EventDef);
        }
    }
}
