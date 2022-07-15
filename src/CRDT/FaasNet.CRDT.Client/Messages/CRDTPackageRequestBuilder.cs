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

        public static CRDTPackage AddGSet(string entityId, List<string> elements, string nonce)
        {
            return new CRDTDeltaPackage
            {
                EntityId = entityId,
                Nonce = nonce,
                Delta = new GSetDelta
                {
                    Elements = elements
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
        public static CRDTPackage Get(string entityId, string nonce)
        {
            return new CRDTGetPackage
            {
                EntityId = entityId,
                Nonce = nonce
            };
        }
    }
}
