using FaasNet.CRDT.Client.Messages;
using FaasNet.CRDT.Client.Messages.Deltas;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.CRDT.Core.Entities
{
    public class CRDTEntityDiff
    {
        public ICollection<CRDTSyncDiffRecordPackage> Diff(CRDTEntity entity, ICollection<ClockValue> clockVector)
        {
            var gCounter = entity as GCounter;
            if (gCounter != null) return Diff(gCounter, clockVector);
            return null;
        }

        public ICollection<CRDTSyncDiffRecordPackage> Diff(GCounter entity, ICollection<ClockValue> clockVector)
        {
            var result = new List<CRDTSyncDiffRecordPackage>();
            var values = entity.ClockVector.Cast<GCounterClockValue>().OrderBy(v => v.Increment);
            var filteredValues = values.Where(v => !clockVector.Any(c => v.ReplicationId == c.ReplicationId && c.Increment >= v.Increment));
            if (filteredValues.Any()) result.AddRange(filteredValues.Select(v => new CRDTSyncDiffRecordPackage
            {
                PeerId = v.ReplicationId,
                Delta = new GCounterDelta(v.Value)
            }));
            return result;
        }
    }
}
