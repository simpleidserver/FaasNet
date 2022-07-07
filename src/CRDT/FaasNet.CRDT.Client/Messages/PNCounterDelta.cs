namespace FaasNet.CRDT.Client.Messages
{
    public class PNCounterDelta : IEntityDelta
    {
        public PNCounterDelta(long change)
        {
            Change = change;
        }

        public long Change { get; set; }
    }
}
