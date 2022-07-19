using System.Threading;

namespace FaasNet.DHT.Chord.Core.Stores
{
    public interface IDHTPeerInfoStore
    {
        DHTPeerInfo Get();
        void Update(DHTPeerInfo peerInfo);
    }

    public class DHTPeerInfoStore : IDHTPeerInfoStore
    {
        private SemaphoreSlim _sharedLock;
        private DHTPeerInfo _peerInfo;

        public DHTPeerInfoStore()
        {
            _sharedLock = new SemaphoreSlim(1);
        }

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
