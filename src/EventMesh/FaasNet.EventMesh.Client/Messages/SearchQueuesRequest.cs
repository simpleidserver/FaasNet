using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class SearchQueuesRequest : BaseEventMeshPackage
    {
        public SearchQueuesRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.SEARCH_QUEUES_REQUEST;
        public FilterQuery Filter { get; set; }

        protected override void SerializeAction(WriteBufferContext context)
        {
            Filter.Serialize(context);
        }

        public SearchQueuesRequest Extract(ReadBufferContext context)
        {
            Filter = new FilterQuery();
            Filter.Deserialize(context);
            return this;
        }
    }
}
