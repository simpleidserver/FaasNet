using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace FaasNet.RaftConsensus.Client
{
    public static class IPAddressHelper
    {
        public static IPAddress ResolveIPAddress(string url)
        {
            var hostEntry = Dns.GetHostEntry(url);
            return hostEntry.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
