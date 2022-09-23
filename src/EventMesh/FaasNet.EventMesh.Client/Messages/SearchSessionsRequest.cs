using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class SearchSessionsRequest : BaseEventMeshPackage
    {
        public SearchSessionsRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.SEARCH_SESSIONS_REQUEST;

        public string ClientId { get; set; }
        public string Vpn { get; set; }
        public FilterQuery Filter { get; set; } = new FilterQuery();

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteString(ClientId);
            context.WriteString(Vpn);
            Filter.Serialize(context);
        }

        public SearchSessionsRequest Extract(ReadBufferContext context)
        {
            ClientId = context.NextString();
            Vpn = context.NextString();
            Filter.Deserialize(context);
            return this;
        }
    }
}
