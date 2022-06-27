using FaasNet.DHT.Chord.Client;

namespace FaasNet.DHT.Chord.Core
{
    public class DHTOptions
    {
        public DHTOptions()
        {
            Url = Constants.DefaultUrl;
            Port = Constants.DefaultPort;
            DimensionFingerTable = 5;
            DimensionSuccessor = 3;
        }

        public string Url { get; set; }
        public int Port { get; set; }
        /// <summary>
        /// m-bit identifier space. 
        /// Peers in Chord are assigned random identifiers ranging from 0 to 2^m-1.
        /// </summary>
        public int DimensionFingerTable { get; set; }
        public int DimensionSuccessor { get; set; }
    }
}
