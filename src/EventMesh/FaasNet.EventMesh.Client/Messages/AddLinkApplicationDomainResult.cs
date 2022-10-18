using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddLinkApplicationDomainResult : BaseEventMeshPackage
    {
        public AddLinkApplicationDomainResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_LINK_APPLICATION_DOMAIN_RESULT;
        public AddLinkApplicationDomainStatus Status { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
        }

        public AddLinkApplicationDomainResult Extract(ReadBufferContext context)
        {
            Status = (AddLinkApplicationDomainStatus)context.NextInt();
            return this;
        }
    }

    public enum AddLinkApplicationDomainStatus
    {
        OK = 0
    }
}
