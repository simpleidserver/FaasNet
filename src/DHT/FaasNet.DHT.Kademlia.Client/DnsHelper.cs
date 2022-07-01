﻿using System.Linq;
using System.Net;

namespace FaasNet.DHT.Kademlia.Client
{
    public class DnsHelper
    {
        public static IPAddress ResolveIPV4(string url)
        {
            var hostEntry = Dns.GetHostEntry(url);
            return hostEntry.AddressList.First(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }
    }
}
