using FaasNet.Peer.Client;

namespace FaasNet.RaftConsensus.Client.StateMachines.Counter
{

    public class GetCounterQueryResult : IQueryResult
    {
        public bool Success { get; private set; }
        public string Id { get; private set; }
        public long Value { get; private set; }


        public static GetCounterQueryResult NotFound() => new GetCounterQueryResult
        {
            Success = false
        };

        public static GetCounterQueryResult OK(string id, long value) => new GetCounterQueryResult
        {
            Success = true,
            Id = id,
            Value = value
        };

        public void Deserialize(ReadBufferContext context)
        {
            Success = context.NextBoolean();
            if (Success)
            {
                Id = context.NextString();
                Value = context.NextLong();
            }
        }

        public void Serialize(WriteBufferContext context)
        {
            context.WriteBoolean(Success);
            if (Success)
            {
                context.WriteString(Id);
                context.WriteLong(Value);
            }
        }
    }
}
