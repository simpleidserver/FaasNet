using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class UpdateEventDefinitionResult : BaseEventMeshPackage
    {
        public UpdateEventDefinitionResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.UPDATE_EVENT_DEFINITION_RESULT;
        public UpdateEventDefinitionStatus Status { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
        }

        public UpdateEventDefinitionResult Extract(ReadBufferContext context)
        {
            Status = (UpdateEventDefinitionStatus)context.NextInt();
            return this;
        }
    }

    public enum UpdateEventDefinitionStatus
    {
        OK = 0,
        NOT_FOUND = 1,
        UNKNOWN_VPN = 2,
        NO_LEADER = 3
    }
}
