using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages.Deltas
{
    public class PNCounterDelta : BaseEntityDelta
    {
        public override EntityDeltaTypes DeltaType => EntityDeltaTypes.PNCounter;

        public PNCounterDelta()
        {

        }

        public PNCounterDelta(long pIncrement, long nIncrement)
        {
            PIncrement = pIncrement;
            NIncrement = nIncrement;
        }

        public long PIncrement { get; set; }
        public long NIncrement { get; set; }

        public override void Serialize(WriteBufferContext context)
        {
            context.WriteLong(PIncrement);
            context.WriteLong(NIncrement);
        }

        public void Extract(ReadBufferContext context)
        {
            PIncrement = context.NextLong();
            NIncrement = context.NextLong();
        }
    }
}
