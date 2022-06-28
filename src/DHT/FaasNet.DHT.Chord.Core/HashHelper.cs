using FaasNet.DHT.Client;
using System;

namespace FaasNet.DHT.Chord.Core
{
    public static class HashHelper
    {
        public static long ComputePeerId(string url, int port, int m)
        {
            long ipNumber = Resolve(url) + port;
            long numberNodes = (long)Math.Pow(m, 2);
            return ipNumber % numberNodes;
        }

		private static long Resolve(string url)
        {
            return DnsHelper.ResolveIPV4(url).Address;
        }
    }
}
