using FaasNet.CRDT.Client.Messages;

namespace FaasNet.CRDT.Core.Entities
{
    public abstract class CRDTEntity<T> where T : IEntityDelta
    {
        public string Id { get; set; }
        public abstract string Name { get; }
        public abstract T ResetAndGetDelta();
        public abstract bool HasDelta { get; }
        public abstract void ApplyDelta(string replicationId, T delta);
    }
}
