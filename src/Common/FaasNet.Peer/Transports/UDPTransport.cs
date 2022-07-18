using FaasNet.Common.Extensions;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Transports
{
    public class UDPTransport : ITransport
    {
        private readonly PeerOptions _options;
        private CancellationTokenSource _cancellationTokenSource;
        private UdpClient _udpServer;

        public UDPTransport(IOptions<PeerOptions> options)
        {
            _options = options.Value;
        }

        public void Start(CancellationToken cancellationToken = default(CancellationToken))
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _udpServer = new UdpClient(new IPEndPoint(IPAddress.Any, _options.Port));
        }

        public void Stop()
        {
            _udpServer.Close();
        }

        public async Task<MessageResult> ReceiveMessage()
        {
            var udpResult = await _udpServer.ReceiveAsync().WithCancellation(_cancellationTokenSource.Token);
            var session = new UDPSession(_udpServer, udpResult, _cancellationTokenSource);
            return new MessageResult(udpResult.Buffer, session.Send);
        }

        public Task Send(byte[] payload, IPEndPoint edp, CancellationToken cancellationToken)
        {
            _udpServer.Send(payload, payload.Length, edp);
            return Task.CompletedTask;
        }

        private class UDPSession
        {
            private readonly UdpClient _udpServer;
            private readonly UdpReceiveResult _udpReceiveResult;
            private readonly CancellationTokenSource _cancellationTokenSource;

            public UDPSession(UdpClient udpServer, UdpReceiveResult udpReceiveResult, CancellationTokenSource cancellationTokenSource)
            {
                _udpServer = udpServer;
                _udpReceiveResult = udpReceiveResult;
                _cancellationTokenSource = cancellationTokenSource;
            }

            public async Task Send(byte[] payload)
            {
                await _udpServer.SendAsync(payload, payload.Count(), _udpReceiveResult.RemoteEndPoint).WithCancellation(_cancellationTokenSource.Token);
            }
        }
    }
}
