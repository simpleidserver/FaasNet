using System;
using System.Linq;
using System.Net;

namespace FaasNet.DHT.Chord.Core
{
    public static class HashHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="port"></param>
        /// <param name="m">m-bit identifier space. Peer are assigned random identifiers ranging from 0 to 2^m - 1</param>
        /// <returns></returns>
        public static long ComputePeerId(string url, int port, int m)
        {
            long ipNumber = Resolve(url) + port;
            long numberNodes = (long)Math.Pow(m, 2);
            return ipNumber % numberNodes;
        }

        private static long Resolve(string url)
        {
            var hostEntry = Dns.GetHostEntry(url);
            var ipv4 = hostEntry.AddressList.First(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            return ipv4.Address;
        }
    }
}
