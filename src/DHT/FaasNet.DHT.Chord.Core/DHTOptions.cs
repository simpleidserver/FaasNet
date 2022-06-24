namespace FaasNet.DHT.Chord.Core
{
    public class DHTOptions
    {
        public DHTOptions()
        {
            Url = "localhost";
            Port = 5000;
            M = 5;
        }

        public string Url { get; set; }
        public int Port { get; set; }
        /// <summary>
        /// m-bit identifier space. 
        /// Peers in Chord are assigned random identifiers ranging from 0 to 2^m-1.
        /// </summary>
        public int M { get; set; }
    }
}
