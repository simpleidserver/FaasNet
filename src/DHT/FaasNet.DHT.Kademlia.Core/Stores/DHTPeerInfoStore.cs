namespace FaasNet.DHT.Kademlia.Core.Stores
{
    public interface IDHTPeerInfoStore
    {
        KademliaPeerInfo Get();
        void Update(KademliaPeerInfo peerInfo);
    }

    public class DHTPeerInfoStore : IDHTPeerInfoStore
    {
        private KademliaPeerInfo _peerInfo;

        public KademliaPeerInfo Get()
        {
            return _peerInfo;
        }

        public void Update(KademliaPeerInfo peerInfo)
        {
            _peerInfo = peerInfo;
        }
    }
}
