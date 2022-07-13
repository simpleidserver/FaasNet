namespace FaasNet.CRDT.Core
{
    public class CRDTProtocolOptions
    {
        public CRDTProtocolOptions()
        {
            SyncCRDTEntitiesTimerMS = 2 * 1000;
            MaxBroadcastedPeers = 4;
        }

        /// <summary>
        /// Sync all CRDT entities every MS.
        /// </summary>
        public int SyncCRDTEntitiesTimerMS { get; set; }
        /// <summary>
        /// Maximum number of Peers targeted by SYNC request.
        /// </summary>
        public int MaxBroadcastedPeers { get; set; }
    }
}
