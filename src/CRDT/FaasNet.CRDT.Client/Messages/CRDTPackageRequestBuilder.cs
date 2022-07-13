using FaasNet.CRDT.Client.Messages.Deltas;
using System.Collections.Generic;

namespace FaasNet.CRDT.Client.Messages
{
    public static class CRDTPackageRequestBuilder
    {
        public static CRDTPackage IncrementGCounter(string entityId, long increment, string nonce)
        {
            return new CRDTDeltaPackage
            {
                EntityId = entityId,
                Nonce = nonce,
                Delta = new GCounterDelta
                {
                    Increment = increment
                }
            };
        }

        public static CRDTPackage Sync(string peerId, string entityId, ICollection<ClockValue> clockVector, string nonce)
        {
            return new CRDTSyncPackage
            {
                PeerId = peerId,
                EntityId = entityId,
                ClockVector = clockVector,
                Nonce = nonce
            };
        }
    }
}
