namespace FaasNet.CRDT.Core
{
    public class CRDTProtocolOptions
    {
        /// <summary>
        /// Sync all CRDT entities every MS.
        /// </summary>
        public int SyncCRDTEntitiesTimerMS { get; set; } = 2 * 1000;
        /// <summary>
        /// Maximum number of Peers targeted by SYNC request.
        /// </summary>
        public int MaxBroadcastedPeers { get; set; } = 4;
        /// <summary>
        /// Expiration time in MS of a request.
        /// </summary>
        public int RequestExpirationTimeMS { get; set; } = 5000;
    }
}
