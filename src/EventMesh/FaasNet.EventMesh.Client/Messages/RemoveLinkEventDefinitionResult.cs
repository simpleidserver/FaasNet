using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class RemoveLinkApplicationDomainResult : BaseEventMeshPackage
    {
        public RemoveLinkApplicationDomainResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.REMOVE_LINK_APPLICATION_DOMAIN_RESULT;
        public RemoveLinkApplicationDomainStatus Status { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
        }

        public RemoveLinkApplicationDomainResult Extract(ReadBufferContext context)
        {
            Status = (RemoveLinkApplicationDomainStatus)context.NextInt();
            return this;
        }
    }

    public enum RemoveLinkApplicationDomainStatus
    {
        OK = 0,
        NOT_FOUND = 1,
        UNKNOWN_VPN = 2,
        NOLEADER = 3
    }
}
