using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Client
{
    public class FindClientsByNameQuery : IQuery
    {
        public string Name { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Name = context.NextString();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Name);
        }
    }
}
