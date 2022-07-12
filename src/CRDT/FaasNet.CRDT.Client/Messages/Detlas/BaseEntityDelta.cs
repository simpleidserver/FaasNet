using FaasNet.Peer.Client;

namespace FaasNet.CRDT.Client.Messages.Deltas
{
    public abstract class BaseEntityDelta
    {
        public abstract EntityDeltaTypes DeltaType { get; }
        public abstract void Serialize(WriteBufferContext context);
    }
}
