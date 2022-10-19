using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddElementApplicationDomainResult : BaseEventMeshPackage
    {
        public AddElementApplicationDomainResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_ELEMENT_APPLICATION_DOMAIN_RESULT;
        public AddElementApplicationDomainStatus Status { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
        }

        public AddElementApplicationDomainResult Extract(ReadBufferContext context)
        {
            Status = (AddElementApplicationDomainStatus)context.NextInt();
            return this;
        }
    }

    public enum AddElementApplicationDomainStatus
    {
        OK = 0,
        UNKNOWN_VPN = 1,
        NOLEADER =  2,
        NOT_FOUND = 3
    }
}
