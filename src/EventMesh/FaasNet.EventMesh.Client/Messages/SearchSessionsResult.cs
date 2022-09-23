using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.EventMesh.Client.StateMachines.Session;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class SearchSessionsResult : BaseEventMeshPackage
    {
        public SearchSessionsResult(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.SEARCH_SESSIONS_RESPONSE;
        public GenericSearchQueryResult<SessionQueryResult> Content { get; set; } = new GenericSearchQueryResult<SessionQueryResult>();

        protected override void SerializeAction(WriteBufferContext context)
        {
            Content.Serialize(context);
        }

        public SearchSessionsResult Extract(ReadBufferContext context)
        {
            Content.Deserialize(context);
            return this;
        }
    }
}
