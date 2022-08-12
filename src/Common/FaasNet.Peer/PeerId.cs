using FaasNet.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace FaasNet.Peer
{
    public record PeerId
    {
        public PeerId(IPEndPoint ipEdp)
        {
            IpEdp = ipEdp;
        }

        public IPEndPoint IpEdp { get; private set; }

        public static PeerId Build(string url, int port)
        {
            return Build(new IPEndPoint(DnsHelper.ResolveIPV4(url), port));
        }

        public static PeerId Build(IPEndPoint ipEdp)
        {
            return new PeerId(ipEdp);
        }

        public string Serialize()
        {
            Span<byte> bytes = stackalloc byte[4];
            IpEdp.Address.TryWriteBytes(bytes, out int length);
            var result = new List<byte>();
            result.AddRange(bytes.ToArray());
            result.AddRange(BitConverter.GetBytes((IpEdp.Port)));
            return Convert.ToHexString(result.ToArray());
        }

        public static PeerId Deserialize(string hex)
        {
            var payload = Convert.FromHexString(hex).ToList();
            var ipEdp = new IPEndPoint(new IPAddress(payload.Take(4).ToArray()), BitConverter.ToInt32(payload.Skip(4).Take(4).ToArray()));
            return new PeerId(ipEdp);
        }
    }
}
