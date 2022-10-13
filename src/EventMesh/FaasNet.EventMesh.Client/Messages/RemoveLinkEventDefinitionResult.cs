using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class RemoveLinkEventDefinitionResult : BaseEventMeshPackage
    {
        public RemoveLinkEventDefinitionResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.REMOVE_LINK_EVENT_DEFINITION_RESULT;
        public RemoveEventDefinitionStatus Status { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
        }

        public RemoveLinkEventDefinitionResult Extract(ReadBufferContext context)
        {
            Status = (RemoveEventDefinitionStatus)context.NextInt();
            return this;
        }
    }

    public enum RemoveEventDefinitionStatus
    {
        OK = 0,
        NOT_FOUND = 1,
        UNKNOWN_VPN = 2,
        NOLEADER = 3
    }
}
