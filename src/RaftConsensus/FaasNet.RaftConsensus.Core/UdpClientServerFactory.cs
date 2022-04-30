using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;

namespace FaasNet.RaftConsensus.Core
{
    public interface IUdpClientServerFactory
    {
        UdpClient Build();
    }

    public class UdpClientServerFactory
    {
        private readonly ConsensusPeerOptions _options;
        private UdpClient _udpClient;

        public UdpClientServerFactory(IOptions<ConsensusPeerOptions> options)
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
