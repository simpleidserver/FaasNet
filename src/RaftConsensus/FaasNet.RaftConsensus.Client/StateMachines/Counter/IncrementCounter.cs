using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.StateMachines.Counter
{
    public class IncrementCounter : ICommand
    {
        public string Id { get; set; }
        public long Value { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Value = context.NextLong();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteString(Id);
            context.WriteLong(Value);
        }
    }
}
