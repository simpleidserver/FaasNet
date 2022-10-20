using FaasNet.EventMesh.Client.Messages;
using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh
{
    public partial class PartitionedEventMeshNode
    {
        public async Task<BaseEventMeshPackage> Handle(GetAllEventDefsRequest getAllEventDefsRequest, CancellationToken cancellationToken)
        {
            var res = await Query<GenericSearchQueryResult<EventDefinitionQueryResult>>(PartitionNames.EVENTDEFINITION_PARTITION_KEY, new GetEventDefsQuery { Filter = getAllEventDefsRequest.Filter }, cancellationToken);
            return PackageResponseBuilder.GetAllEventDefs(getAllEventDefsRequest.Seq, res);
        }
    }
}
