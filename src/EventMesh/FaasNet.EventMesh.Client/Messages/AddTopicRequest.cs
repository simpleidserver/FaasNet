using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddTopicRequest : BaseEventMeshPackage
    {
        public AddTopicRequest(string seq) : base(seq)
        {
        }
        public AddTopicRequest(string seq, string topic) : base(seq)
        {
            Topic = topic;
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_TOPIC_REQUEST;

        public string Topic { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Topic);
        }

        public AddTopicRequest Extract(ReadBufferContext context)
        {
            Topic = context.NextString();
            return this;
        }
    }
}
