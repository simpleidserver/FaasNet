using FaasNet.CRDT.Client.Messages;
using FaasNet.CRDT.Client.Messages.Deltas;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CRDT.Core.Entities
{
    public class GSet : CRDTEntity
    {
        private object _lock = new object();
        public static string NAME = "GSet";
        public override string Name => NAME;
        private readonly string _replicationId;
        private readonly ICollection<GSetClockValue> _clockVector;

        public GSet(string replicationId)
        {
            _replicationId = replicationId;
            _clockVector = new List<GSetClockValue>
            {
                new GSetClockValue { Increment = 0, Elements = new List<string>(), ReplicationId = replicationId }
            };
        }

        public GSet(string replicationId, ICollection<GSetClockValue> clockVector)
        {
            _replicationId = replicationId;
            _clockVector = clockVector;
        }

        public override object Value => _clockVector.GroupBy(c => c.ReplicationId).SelectMany(c => c).SelectMany(c => c.Elements);

        public override ICollection<ClockValue> ClockVector => _clockVector.Cast<ClockValue>().ToList();

        public override void ApplyDelta(string replicationId, BaseEntityDelta delta)
        {
            lock(_lock)
            {
                var gset = delta as GSetDelta;
                var clockValue = _clockVector.OrderByDescending(c => c.Increment).FirstOrDefault(c => c.ReplicationId == replicationId);
                GSetClockValue newClockValue = null;
                if (clockValue == null)
                {
                    newClockValue = new GSetClockValue { Increment = 0, ReplicationId = replicationId, Elements = gset.Elements };
                    _clockVector.Add(newClockValue);
                    return;
                }

                newClockValue = new GSetClockValue { Increment = clockValue.Increment + 1, ReplicationId = replicationId, Elements = gset.Elements };
                _clockVector.Add(newClockValue);
            }
        }

        public GSet Add(List<string> value)
        {
            var clockValue = _clockVector.OrderByDescending(c => c.Increment).FirstOrDefault(c => c.ReplicationId == _replicationId);
            GSetClockValue newClockValue = null;
            if(clockValue == null)
            {
                newClockValue = new GSetClockValue { Increment = 0, ReplicationId = _replicationId, Elements = value };
                _clockVector.Add(newClockValue);
                return this;
            }

            newClockValue = new GSetClockValue { Increment = clockValue.Increment + 1, ReplicationId = _replicationId, Elements = value };
            _clockVector.Add(newClockValue);
            return this;
        }
    }

    public class GSetClockValue : ClockValue
    {
        public ICollection<string> Elements { get; set; }
    }
}
