using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Session
{
    public class GetSessionQuery : IQuery
    {
        public string Id { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
        }
    }
}
