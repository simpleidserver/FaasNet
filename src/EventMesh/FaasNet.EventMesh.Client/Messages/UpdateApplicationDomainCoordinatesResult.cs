using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class UpdateApplicationDomainCoordinatesResult : BaseEventMeshPackage
    {
        public UpdateApplicationDomainCoordinatesResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.UPDATE_APPLICATION_DOMAIN_COORDINATES_RESULT;
        public UpdateApplicationDomainCoordinatesStatus Status { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
        }

        public UpdateApplicationDomainCoordinatesResult Extract(ReadBufferContext context)
        {
            Status = (UpdateApplicationDomainCoordinatesStatus)context.NextInt();
            return this;
        }
    }

    public enum UpdateApplicationDomainCoordinatesStatus
    {
        OK = 0,
        UNKNOWN_VPN = 1,
        NOLEADER
    }
}
