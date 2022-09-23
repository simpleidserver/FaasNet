using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Queue;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class SearchQueuesResult : BaseEventMeshPackage
    {
        public SearchQueuesResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.SEARCH_QUEUES_RESPONSE;
        public GenericSearchQueryResult<QueueQueryResult> Content { get; set; } = new GenericSearchQueryResult<QueueQueryResult>();

        protected override void SerializeAction(WriteBufferContext context)
        {
            Content.Serialize(context);
        }

        public SearchQueuesResult Extract(ReadBufferContext context)
        {
            Content = new GenericSearchQueryResult<QueueQueryResult>();
            Content.Deserialize(context);
            return this;
        }
    }
}
