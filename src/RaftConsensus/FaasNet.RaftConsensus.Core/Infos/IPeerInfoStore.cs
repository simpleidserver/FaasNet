namespace FaasNet.RaftConsensus.Core.Infos
{
    public interface IPeerInfoStore
    {
        PeerInfo Get();
    }

    public class PeerInfoStore : IPeerInfoStore
    {
        private readonly PeerInfo _currentPeerInfo;

        public PeerInfoStore()
        {
            _currentPeerInfo = new PeerInfo();
        }

        public PeerInfo Get()
        {
            return _currentPeerInfo;
        }
    }
}
