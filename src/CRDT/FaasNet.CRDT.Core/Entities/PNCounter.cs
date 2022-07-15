using FaasNet.CRDT.Client.Messages;
using FaasNet.CRDT.Client.Messages.Deltas;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CRDT.Core.Entities
{
    public class PNCounter : CRDTEntity
    {
        private object _lock = new object();
        private readonly string _replicationId;
        public const string NAME = "PNCounter";
        public override string Name => NAME;
        private readonly ICollection<PNCounterClockValue> _clockVector;

        public PNCounter(string replicationId)
        {
            _replicationId = replicationId;
            _clockVector = new List<PNCounterClockValue>
            {
                new PNCounterClockValue { Increment = 0, NValue = 0, PValue = 0, ReplicationId = replicationId }
            };
        }

        public PNCounter(string replicationId, ICollection<PNCounterClockValue> clockVector)
        {
            _replicationId = replicationId;
            _clockVector = clockVector;
        }

        public override ICollection<ClockValue> ClockVector => _clockVector.Cast<ClockValue>().ToList();
        public override object Value => _clockVector.GroupBy(c => c.ReplicationId).SelectMany(c => c).Sum(c => c.PValue) - _clockVector.GroupBy(c => c.ReplicationId).SelectMany(c => c).Sum(c => c.NValue);

        public override void ApplyDelta(string replicationId, BaseEntityDelta delta)
        {
            lock(_lock)
            {
                var counter = delta as PNCounterDelta;
                var clockValue = _clockVector.OrderByDescending(c => c.Increment).FirstOrDefault(c => c.ReplicationId == replicationId);
                PNCounterClockValue newClockValue = null;
                if (clockValue == null)
                {
                    newClockValue = new PNCounterClockValue { Increment = 0, ReplicationId = replicationId, PValue = counter.PIncrement, NValue = counter.NIncrement };
                    _clockVector.Add(newClockValue);
                    return;
                }

                newClockValue = new PNCounterClockValue { Increment = clockValue.Increment + 1, ReplicationId = replicationId, PValue = counter.PIncrement, NValue = counter.NIncrement };
                _clockVector.Add(newClockValue);
            }
        }

        public PNCounter Increment()
        {
            var clockValue = _clockVector.OrderByDescending(c => c.Increment).FirstOrDefault(c => c.ReplicationId == _replicationId);
            PNCounterClockValue newClockValue = null;
            if (clockValue == null)
            {
                newClockValue = new PNCounterClockValue { Increment = 0, ReplicationId = _replicationId, PValue = 1 };
                _clockVector.Add(newClockValue);
                return this;
            }

            newClockValue = new PNCounterClockValue { Increment = clockValue.Increment + 1, ReplicationId = _replicationId, PValue = 1 };
            _clockVector.Add(newClockValue);
            return this;
        }

        public PNCounter Decrement()
        {
            var clockValue = _clockVector.OrderByDescending(c => c.Increment).FirstOrDefault(c => c.ReplicationId == _replicationId);
            PNCounterClockValue newClockValue = null;
            if (clockValue == null)
            {
                newClockValue = new PNCounterClockValue { Increment = 0, ReplicationId = _replicationId, NValue = 1 };
                _clockVector.Add(newClockValue);
                return this;
            }

            newClockValue = new PNCounterClockValue { Increment = clockValue.Increment + 1, ReplicationId = _replicationId, NValue = 1 };
            _clockVector.Add(newClockValue);
            return this;
        }
    }

    public class PNCounterClockValue : ClockValue
    {
        public long PValue { get; set; }
        public long NValue { get; set; }
    }
}
