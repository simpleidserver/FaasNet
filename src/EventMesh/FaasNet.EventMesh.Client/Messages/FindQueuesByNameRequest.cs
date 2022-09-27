using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class FindQueuesByNameRequest : BaseEventMeshPackage
    {
        public FindQueuesByNameRequest(string seq) : base(seq)
        {
        }

        public string Name { get; set; }
        public override EventMeshCommands Command => EventMeshCommands.FIND_QUEUES_BY_NAME_REQUEST;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(Name);
        }

        public FindQueuesByNameRequest Extract(ReadBufferContext context)
        {
            Name = context.NextString();
            return this;
        }
    }
}
