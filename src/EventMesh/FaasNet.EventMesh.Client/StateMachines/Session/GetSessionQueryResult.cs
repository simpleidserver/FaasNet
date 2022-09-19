using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Session
{
    public class GetSessionQueryResult : IQueryResult
    {
        public GetSessionQueryResult()
        {
            Success = false;
        }

        public GetSessionQueryResult(SessionQueryResult session)
        {
            Success = true;
            Session = session;
        }

        public bool Success { get; set; }
        public SessionQueryResult Session { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if(Success)
            {
                Session = new SessionQueryResult();
                Session.Deserialize(context);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
            if(Success) Session.Serialize(context);
        }
    }
}
