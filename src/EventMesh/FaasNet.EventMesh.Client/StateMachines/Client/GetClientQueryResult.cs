using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Client
{
    public class GetClientQueryResult : IQueryResult
    {
        public GetClientQueryResult()
        {
            Success = false;
        }

        public GetClientQueryResult(ClientQueryResult message)
        {
            Success = true;
            Client = message;
        }

        public bool Success { get; set; }
        public ClientQueryResult Client { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if (Success)
            {
                Client = new ClientQueryResult();
                Client.Deserialize(context);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
            if (Success) Client.Serialize(context);
        }
    }
}
