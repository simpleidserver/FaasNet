using FaasNet.Common.Extensions;
using FaasNet.Common.Helpers;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Client.Transports
{
    public class ClientUDPTransport : IClientTransport
    {
        private IPEndPoint _target;
        private UdpClient _udpClient;

        public string Name => "UDP";

        public void Open(string url, int port)
        {
            Open(new IPEndPoint(DnsHelper.ResolveIPV4(url), port));
        }

        public void Open(IPEndPoint edp)
        {
            _target = edp;
            _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        }

        public async Task<byte[]> Receive(int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var receivedResult = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken, timeoutMS);
            return receivedResult.Buffer;
        }

        public async Task<int> Send(byte[] payload, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await _udpClient.SendAsync(payload, payload.Count(), _target).WithCancellation(cancellationToken, timeoutMS);
            return result;
        }

        public void Close()
        {
            _udpClient?.Close();
            _udpClient.Dispose();
            _udpClient = null;
        }

        public void Dispose()
        {
            Close();
        }

        public IClientTransport CloneAndOpen()
        {
            var result = new ClientUDPTransport();
            result.Open(_target);
            return result;
        }
    }
}
