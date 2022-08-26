using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.Commands
{
    public class IncrementGCounterCommand : ICommand
    {
        public long Value { get; set; }

        public void Deserialize(ReadBufferContext context)
        {
            Value = context.NextLong();
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteLong(Value);
        }
    }
}
