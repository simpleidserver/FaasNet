using FaasNet.DHT.Chord.Core;

namespace FaasNet.DHT.Chord.Service
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var tt = HashHelper.ComputePeerId("localhost", 5000, 5);
            var tt2 = HashHelper.ComputePeerId("localhost", 5001, 5);
            var tt3 = HashHelper.ComputePeerId("localhost", 5003, 5);
            return 1;
        }
    }
}
