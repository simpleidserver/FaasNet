using FaasNet.CRDT.Client.Messages;
using FaasNet.CRDT.Client.Messages.Deltas;
using System.Collections.Generic;

namespace FaasNet.CRDT.Core.Entities
{
    public abstract class CRDTEntity
    {
        public abstract string Name { get; }
        public abstract bool HasDelta { get; }
        public abstract void ApplyDelta(string replicationId, BaseEntityDelta delta);
        public abstract ICollection<ClockValue> ClockVector { get; }
    }
}
