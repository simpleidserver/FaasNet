using FaasNet.EventMesh.Client.StateMachines.EventDefinition;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetEventDefinitionResult : BaseEventMeshPackage
    {
        public GetEventDefinitionResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_EVENT_DEFINITION_RESULT;
        public GetEventDefinitionStatus Status { get; set; }
        public EventDefinitionQueryResult Result { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
            Result.Serialize(context);
        }

        public GetEventDefinitionResult Extract(ReadBufferContext context)
        {
            Status = (GetEventDefinitionStatus)context.NextInt();
            if (Status == GetEventDefinitionStatus.OK)
            {
                Result = new EventDefinitionQueryResult();
                Result.Deserialize(context);
            }

            return this;
        }
    }

    public enum GetEventDefinitionStatus
    {
        OK = 0,
        NOT_FOUND = 1
    }
}
