using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllEventDefsResult : BaseEventMeshPackage
    {
        public GetAllEventDefsResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_ALL_EVENT_DEFS_RESULT;
        public GenericSearchQueryResult<EventDefinitionQueryResult> Content { get; set; } = new GenericSearchQueryResult<EventDefinitionQueryResult>();

        protected override void SerializeAction(WriteBufferContext context)
        {
            Content.Serialize(context);
        }

        public GetAllEventDefsResult Extract(ReadBufferContext context)
        {
            Content.Deserialize(context);
            return this;
        }
    }
}
