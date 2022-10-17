using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllApplicationDomainsRequest : BaseEventMeshPackage
    {
        public GetAllApplicationDomainsRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_ALL_APPLICATION_DOMAINS_REQUEST;
        public FilterQuery Filter { get; set; } = new FilterQuery();

        protected override void SerializeAction(WriteBufferContext context)
        {
            Filter.Serialize(context);
        }

        public GetAllApplicationDomainsRequest Extract(ReadBufferContext context)
        {
            Filter = new FilterQuery();
            Filter.Deserialize(context);
            return this;
        }
    }
}
