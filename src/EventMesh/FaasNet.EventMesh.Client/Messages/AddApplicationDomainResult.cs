using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddApplicationDomainResult : BaseEventMeshPackage
    {
        public AddApplicationDomainResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_APPLICATION_DOMAIN_RESULT;
        public AddApplicationDomainStatus Status { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
        }

        public AddApplicationDomainResult Extract(ReadBufferContext context)
        {
            Status = (AddApplicationDomainStatus)context.NextInt();
            return this;
        }
    }

    public enum AddApplicationDomainStatus
    {
        OK = 0,
        UNKNOWN_VPN = 1,
        NOLEADER = 2
    }
}
