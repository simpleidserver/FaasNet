using FaasNet.CRDT.Client.Messages;

namespace FaasNet.CRDT.Core
{
    public interface ICRDTEntity<T> where T : IEntityDelta
    {
        string Name { get; }
        T Delta { get; }
        bool HasDelta { get; }
        void ApplyDelta(T delta);
    }
}
