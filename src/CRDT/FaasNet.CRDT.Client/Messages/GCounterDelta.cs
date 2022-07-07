namespace FaasNet.CRDT.Client.Messages
{
    public class GCounterDelta : IEntityDelta
    {
        public GCounterDelta(long increment)
        {
            Increment = increment;
        }

        public long Increment { get; set; }
    }
}
