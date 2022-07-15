using FaasNet.Peer.Client;
namespace FaasNet.CRDT.Client.Messages.Deltas
{
    public class GCounterDelta : BaseEntityDelta
    {
        public override EntityDeltaTypes DeltaType => EntityDeltaTypes.GCounter;

        public GCounterDelta() { }

        public GCounterDelta(long increment)
        {
            Increment = increment;
        }

        public long Increment { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            context.WriteLong(Increment);
        }

        public void Extract(ReadBufferContext context)
        {
            Increment = context.NextLong();
        }
    }
}
