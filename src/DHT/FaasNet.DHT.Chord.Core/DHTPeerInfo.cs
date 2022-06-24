namespace FaasNet.DHT.Chord.Core
{
    public class DHTPeerInfo
    {
        public DHTPeerInfo(string url, int port, int m)
        {
            Id = HashHelper.ComputePeerId(url, port, m);
        }

        public long Id { get; private set; }
    }
}
