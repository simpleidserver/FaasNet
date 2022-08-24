using FaasNet.Common.Extensions;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Transports
{
    public class ServerUDPTransport : IServerTransport
    {
        private readonly PeerOptions _options;
        private CancellationTokenSource _cancellationTokenSource;
        private UdpClient _udpServer;

        public ServerUDPTransport(IOptions<PeerOptions> options)
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

        public async Task<BaseSessionResult> ReceiveSession()
        {
            var udpResult = await _udpServer.ReceiveAsync().WithCancellation(_cancellationTokenSource.Token);
            return new UDPSessionResult(udpResult, _udpServer, _cancellationTokenSource);
        }

        public Task Send(byte[] payload, IPEndPoint edp, CancellationToken cancellationToken)
        {
            _udpServer.Send(payload, payload.Length, edp);
            return Task.CompletedTask;
        }

        public async Task<byte[]> Receive(CancellationToken cancellationToken)
        {
            var udpResult = await _udpServer.ReceiveAsync().WithCancellation(_cancellationTokenSource.Token);
            return udpResult.Buffer;
        }

        private class UDPSessionResult : BaseSessionResult
        {
            private readonly UdpReceiveResult _udpReceiveResult;
            private readonly UdpClient _udpServer;
            private readonly CancellationTokenSource _cancellationTokenSource;

            public UDPSessionResult(UdpReceiveResult udpReceiveResult, UdpClient udpServer, CancellationTokenSource cancellationTokenSource)
            {
                _udpReceiveResult = udpReceiveResult;
                _udpServer = udpServer;
                _cancellationTokenSource = cancellationTokenSource;
            }

            public override Task<byte[]> ReceiveMessage()
            {
                var result = _udpReceiveResult.Buffer;
                return Task.FromResult(result);
            }

            public override async Task SendMessage(byte[] payload)
            {
                await _udpServer.SendAsync(payload, payload.Count(), _udpReceiveResult.RemoteEndPoint).WithCancellation(_cancellationTokenSource.Token);
            }
        }
    }
}
