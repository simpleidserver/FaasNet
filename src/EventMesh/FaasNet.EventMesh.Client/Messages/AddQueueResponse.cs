using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddQueueResponse : BaseEventMeshPackage
    {
        public AddQueueResponse(string seq) : base(seq)
        {
            Status = AddQueueStatus.SUCCESS;
        }

        public AddQueueResponse(string seq, AddQueueStatus status) : this(seq)
        {
            Status = status;
        }

        public AddQueueStatus Status { get; set; }

        public override EventMeshCommands Command => EventMeshCommands.ADD_QUEUE_RESPONSE;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
        }

        public AddQueueResponse Extract(ReadBufferContext context)
        {
            Status = (AddQueueStatus)context.NextInt();
            return this;
        }
    }

    public enum AddQueueStatus
    {
        SUCCESS = 0,
        NOT_ENOUGHT_ACTIVENODES = 1,
        INTERNAL_ERROR = 2,
        EXISTING_QUEUE = 3,
        UNKNOWN_VPN = 4
    }
}
