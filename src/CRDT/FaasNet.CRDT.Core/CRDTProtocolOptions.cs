namespace FaasNet.CRDT.Core
{
    public class CRDTProtocolOptions
    {
        public CRDTProtocolOptions()
        {
            SyncCRDTEntitiesTimerMS = 2 * 1000;
            MaxBroadcastedPeers = 4;
        }

        public int SyncCRDTEntitiesTimerMS { get; set; }
        public int MaxBroadcastedPeers { get; set; }
    }
}
