using FaasNet.CRDT.Client.Messages.Deltas;

namespace FaasNet.CRDT.Client.Messages
{
    public static class CRDTPackageRequestBuilder
    {
        public static CRDTPackage IncrementGCounter(string peerId, string entityId, long increment, string nonce)
        {
            return new CRDTDeltaPackage
            {
                PeerId = peerId,
                EntityId = entityId,
                Nonce = nonce,
                Delta = new GCounterDelta
                {
                    Increment = increment
                }
            };
        }
    }
}
