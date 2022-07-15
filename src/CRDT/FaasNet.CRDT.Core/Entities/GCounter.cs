using FaasNet.CRDT.Client.Messages;
using FaasNet.CRDT.Client.Messages.Deltas;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CRDT.Core.Entities
{
    public class GCounter : CRDTEntity
    {
        private object _lock = new object();
        public static string NAME = "GCounter";
        private readonly string _replicationId;
        private readonly ICollection<GCounterClockValue> _clockVector;

        public GCounter(string replicationId)
        {
            _replicationId = replicationId;
            _clockVector = new List<GCounterClockValue>
            {
                new GCounterClockValue { Increment = 0, Value = 0, ReplicationId = _replicationId }
            };
        }

        public GCounter(string replicationId, ICollection<GCounterClockValue> clockVector)
        {
            _replicationId = replicationId;
            _clockVector = clockVector;
        }

        public override string Name => NAME;
        public override object Value => _clockVector.GroupBy(c => c.ReplicationId).SelectMany(c => c).Sum(c => c.Value);
        public override ICollection<ClockValue> ClockVector => _clockVector.Cast<ClockValue>().ToList();

        public override void ApplyDelta(string replicationId, BaseEntityDelta delta)
        {
            lock(_lock)
            {
                var counter = delta as GCounterDelta;
                var clockValue = _clockVector.OrderByDescending(c => c.Increment).FirstOrDefault(c => c.ReplicationId == replicationId);
                GCounterClockValue newClockValue = null;
                if (clockValue == null)
                {
                    newClockValue = new GCounterClockValue { Increment = 0, ReplicationId = replicationId, Value = counter.Increment };
                    _clockVector.Add(newClockValue);
                    return;
                }

                newClockValue = new GCounterClockValue { Increment = clockValue.Increment + 1, ReplicationId = replicationId, Value = counter.Increment };
                _clockVector.Add(newClockValue);
            }
        }

        public GCounter Increment()
        {
            var clockValue = _clockVector.OrderByDescending(c => c.Increment).FirstOrDefault(c => c.ReplicationId == _replicationId);
            GCounterClockValue newClockValue = null;
            if (clockValue == null)
            {
                newClockValue = new GCounterClockValue { Increment = 0, ReplicationId = _replicationId, Value = 1 };
                _clockVector.Add(newClockValue);
                return this;
            }

            newClockValue = new GCounterClockValue { Increment = clockValue.Increment + 1, ReplicationId = _replicationId, Value = 1 };
            _clockVector.Add(newClockValue);
            return this;
        }
    }

    public class GCounterClockValue : ClockValue
    {
        public long Value { get; set; }
    }
}
