using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.ApplicationDomain
{
    public class GetApplicationDomainQueryResult : IQueryResult
    {
        public bool Success { get; set; }
        public ApplicationDomainQueryResult Content { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if(Success)
            {
                Content = new ApplicationDomainQueryResult();
                Content.Deserialize(context);
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
            if(Success)
            {
                Content.Serialize(context);
            }
        }
    }
}
