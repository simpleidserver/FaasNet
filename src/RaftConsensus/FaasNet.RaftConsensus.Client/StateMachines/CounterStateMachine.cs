using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.StateMachines
{
    public class CounterStateMachine : IStateMachine
    {
        public string Id { get; set; }
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

        public void Deserialize(ReadBufferContext context)
        {
            Id = context.NextString();
            Value = context.NextLong();
        }

        public byte[] Serialize()
        {
            var result = new WriteBufferContext();
            result.WriteString(Id).WriteLong(Value);
            return result.Buffer.ToArray();
        }
    }

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
