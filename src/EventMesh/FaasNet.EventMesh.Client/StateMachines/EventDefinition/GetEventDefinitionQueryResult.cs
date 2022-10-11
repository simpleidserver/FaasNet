using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.EventDefinition
{
    public class GetEventDefinitionQueryResult : IQueryResult
    {
        public GetEventDefinitionQueryResult()
        {
            Success = false;
        }

        public GetEventDefinitionQueryResult(EventDefinitionQueryResult eventDef)
        {
            Success = true;
            EventDef = eventDef;
        }

        public bool Success { get; set; }
        public EventDefinitionQueryResult EventDef { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if (Success)
            {
                EventDef = new EventDefinitionQueryResult();
                EventDef.Deserialize(context);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
            if (Success) EventDef.Serialize(context);
        }
    }
}
