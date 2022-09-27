using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;

namespace FaasNet.EventMesh.Client.StateMachines.Queue
{
    public class FindQueuesByNameQuery : IQuery
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
