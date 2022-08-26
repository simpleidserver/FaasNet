using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Commands;

namespace FaasNet.RaftConsensus.Core.StateMachines
{
    public class GCounter : IStateMachine
    {
        public long Value { get; set; }

        public void Apply(ICommand cmd)
        {
            switch(cmd)
            {
                case IncrementGCounterCommand increment:
                    Value += increment.Value;
                    break;
            }
        }

        public byte[] Serialize()
        {
            var context = new WriteBufferContext();
            context.WriteLong(Value);
            return context.Buffer.ToArray();
        }

        public void Deserialize(ReadBufferContext context)
        {
            Value = context.NextLong();
        }
    }
}
