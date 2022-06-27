namespace FaasNet.DHT.Chord.Core.Stores
{
    public interface IDHTPeerInfoStore
    {
        DHTPeerInfo Get();
        void Update(DHTPeerInfo peerInfo);
    }

    public class DHTPeerInfoStore : IDHTPeerInfoStore
    {
        private DHTPeerInfo _peerInfo;

        public DHTPeerInfo Get()
        {
            return _peerInfo;
        }

        public void Update(DHTPeerInfo peerInfo)
        {
            _peerInfo = peerInfo;
        }
    }
}
