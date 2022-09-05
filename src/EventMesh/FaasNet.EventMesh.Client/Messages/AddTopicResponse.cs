using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddTopicResponse : BaseEventMeshPackage
    {
        public AddTopicResponse(string seq) : base(seq)
        {
            Status = AddTopicStatus.SUCCESS;
        }

        public AddTopicResponse(string seq, AddTopicStatus status) : this(seq)
        {
            Status = status;
        }

        public AddTopicStatus Status { get; set; }

        public override EventMeshCommands Command => EventMeshCommands.ADD_TOPIC_RESPONSE;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteInteger((int)Status);
        }

        public AddTopicResponse Extract(ReadBufferContext context)
        {
            Status = (AddTopicStatus)context.NextInt();
            return this;
        }
    }

    public enum AddTopicStatus
    {
        SUCCESS = 0,
        EXISTING_TOPIC = 1,
        NOT_ENOUGHT_ACTIVENODES = 2,
        INTERNAL_ERROR = 3
    }
}
