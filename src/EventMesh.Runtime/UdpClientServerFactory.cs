using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;

namespace EventMesh.Runtime
{
    public class UdpClientServerFactory : IUdpClientServerFactory
    {
        private readonly RuntimeOptions _options;
        private UdpClient _udpClient;

        public UdpClientServerFactory(IOptions<RuntimeOptions> options)
        {
            _options = options.Value;
        }

        public UdpClient Build()
        {
            if (_udpClient == null)
            {
                _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, _options.Port));
            }

            return _udpClient;
        }
    }
}