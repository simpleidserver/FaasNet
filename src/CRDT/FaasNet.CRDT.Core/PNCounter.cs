using FaasNet.CRDT.Client.Messages;

namespace FaasNet.CRDT.Core
{
    public class PNCounter : ICRDTEntity<PNCounterDelta>
    {
        private long _value;
        private long _deltaValue;

        public string Name => "PNCounter";
        public PNCounterDelta Delta => new PNCounterDelta(_deltaValue);
        public bool HasDelta => _deltaValue != default(long);

        public long Decrement()
        {
            _value -= 1;
            _deltaValue -= 1;
            return _value;
        }

        public long Increment()
        {
            _value += 1;
            _deltaValue += 1;
            return _value;
        }

        public void ApplyDelta(PNCounterDelta delta)
        {
            _value += delta.Change;
        }
    }
}
