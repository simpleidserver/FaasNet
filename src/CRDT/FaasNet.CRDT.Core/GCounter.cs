using FaasNet.CRDT.Client.Messages;

namespace FaasNet.CRDT.Core
{
    public class GCounter : ICRDTEntity<GCounterDelta>
    {
        private long _deltaValue;
        private long _value;

        public GCounter()
        {
            _deltaValue = 0;
            _value = 0;
        }

        public string Name => "GCounter";
        public GCounterDelta Delta => new GCounterDelta(_deltaValue);
        public bool HasDelta => _deltaValue == default(long);

        public long Increment()
        {
            _deltaValue++;
            _value++;
            return _value;
        }

        public void ResetDelta()
        {
            _deltaValue = 0;
        }

        public void ApplyDelta(GCounterDelta delta)
        {
            _value += delta.Increment;
        }
    }
}
