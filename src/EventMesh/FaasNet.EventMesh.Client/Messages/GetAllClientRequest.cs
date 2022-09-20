using FaasNet.EventMesh.Client.StateMachines;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetAllClientRequest : BaseEventMeshPackage
    {
        public GetAllClientRequest(string seq) : base(seq)
        {
        }

        public override EventMeshCommands Command => EventMeshCommands.GET_ALL_CLIENT_REQUEST;

        public FilterQuery Filter { get; set; } = new FilterQuery();

        protected override void SerializeAction(WriteBufferContext context)
        {
            Filter.Serialize(context);
        }

        public GetAllClientRequest Extract(ReadBufferContext context)
        {
            Filter = new FilterQuery();
            Filter.Deserialize(context);
            return this;
        }
    }
}
