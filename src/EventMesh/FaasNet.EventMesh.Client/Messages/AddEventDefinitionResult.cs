using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddEventDefinitionResult : BaseEventMeshPackage
    {
        public AddEventDefinitionResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_EVENT_DEFINITION_RESPONSE;
        public string EventDefinitionId { get; set; }
        public AddEventDefinitionStatus Status { get; set; }
        public long? Term { get; set; } = null;
        public long? MatchIndex { get; set; } = null;
        public long? LastIndex { get; set; } = null;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(EventDefinitionId);
            context.WriteInteger((int)Status);
            if(Status == AddEventDefinitionStatus.OK)
            {
                context.WriteLong(Term.Value);
                context.WriteLong(MatchIndex.Value);
                context.WriteLong(LastIndex.Value);
            }
        }

        public AddEventDefinitionResult Extract(ReadBufferContext context)
        {
            EventDefinitionId = context.NextString();
            Status = (AddEventDefinitionStatus)context.NextInt();
            if (Status == AddEventDefinitionStatus.OK)
            {
                Term = context.NextLong();
                MatchIndex = context.NextLong();
                LastIndex = context.NextLong();
            }

            return this;
        }
    }

    public enum AddEventDefinitionStatus
    {
        OK = 0,
        UNKNOWN_VPN = 1,
        EXISTING_EVENTDEFINITION = 2,
        UNKNOWN_SOURCE = 3,
        UNKNOWN_TARGET = 4,
        NOLEADER = 3
    }
}
