using FaasNet.CRDT.Client.Messages;
using FaasNet.CRDT.Client.Messages.Deltas;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CRDT.Core.Entities
{
    public class CRDTEntityDiff
    {
        public ICollection<BaseEntityDelta> Diff(CRDTEntity entity, ICollection<ClockValue> clockVector)
        {
            var gCounter = entity as GCounter;
            if (gCounter != null) return Diff(gCounter, clockVector);
            return null;
        }

        public ICollection<BaseEntityDelta> Diff(GCounter entity, ICollection<ClockValue> clockVector)
        {
            var result = new List<BaseEntityDelta>();
            var values = entity.ClockVector.Cast<GCounterClockValue>().OrderBy(v => v.Increment);
            foreach (var clockValue in clockVector)
            {
                var filteredValues = values.Where(v => v.ReplicationId == clockValue.ReplicationId && v.Increment > clockValue.Increment);
                if(filteredValues.Any()) result.AddRange(filteredValues.Select(v => new GCounterDelta(v.Value)));
            }

            return result;
        }
    }
}
