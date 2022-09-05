using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class AddTopicRequest : BaseEventMeshPackage
    {
        public AddTopicRequest(string seq) : base(seq)
        {
        }
        public AddTopicRequest(string seq, string topic, bool isBroadcasted) : base(seq)
        {
            Topic = topic;
            IsBroadcasted = isBroadcasted;
        }

        public override EventMeshCommands Command => EventMeshCommands.ADD_TOPIC_REQUEST;

        public string Topic { get; set; }
        public bool IsBroadcasted { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Topic);
            context.WriteBoolean(IsBroadcasted);
        }

        public AddTopicRequest Extract(ReadBufferContext context)
        {
            Topic = context.NextString();
            IsBroadcasted = context.NextBoolean();
            return this;
        }
    }
}
