using FaasNet.CRDT.Client.Messages.Deltas;

namespace FaasNet.CRDT.Core.Entities
{
    public abstract class CRDTEntity
    {
        public abstract string Name { get; }
        public abstract BaseEntityDelta ResetAndGetDelta();
        public abstract bool HasDelta { get; }
        public abstract void ApplyDelta(string replicationId, BaseEntityDelta delta);
    }
}
