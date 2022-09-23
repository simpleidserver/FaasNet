using FaasNet.EventMesh.Client.StateMachines.Client;
using FaasNet.Peer.Client;

namespace FaasNet.EventMesh.Client.Messages
{
    public class GetClientResult : BaseEventMeshPackage
    {
        public GetClientResult(string seq) : base(seq)
        {
        }

        public bool Success { get; set; }
        public ClientQueryResult Content { get; set; }
        public override EventMeshCommands Command => EventMeshCommands.GET_CLIENT_RESPONSE;

        protected override void SerializeAction(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
            if(Success) Content.Serialize(context);
        }

        public GetClientResult Extract(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if (Success)
            {
                Content = new ClientQueryResult();
                Content.Deserialize(context);
            }

            return this;
        }
    }
}
