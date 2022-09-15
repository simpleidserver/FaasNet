using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.StateMachines.Counter
{
    public class GetCounterQuery : IQuery
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
