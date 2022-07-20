using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Peer.Transports
{
    public class TCPTransport : ITransport
    {
        private readonly PeerOptions _options;
        private TcpListener _tcpServer;
        private CancellationTokenSource _cancellationTokenSource;

        public TCPTransport(IOptions<PeerOptions> options)
        {
            _options = options.Value;
        }

        public void Start(CancellationToken cancellationToken = default(CancellationToken))
        {
            _tcpServer = new TcpListener(new IPEndPoint(IPAddress.Any, _options.Port));
            _tcpServer.Start();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        public void Stop()
        {
            _tcpServer.Stop();
        }

        public async Task<BaseSessionResult> ReceiveSession()
        {
            var tcpClient = await _tcpServer.AcceptTcpClientAsync(_cancellationTokenSource.Token);
            BaseSessionResult result = new TCPSessionResult(tcpClient, _cancellationTokenSource);
            return result;
        }

        public Task Send(byte[] payload, IPEndPoint edp, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private class TCPSessionResult : BaseSessionResult
        {
            private readonly TcpClient _tcpClient;
            private readonly CancellationTokenSource _cancellationTokenSource;
            private readonly NetworkStream _ns;

            public TCPSessionResult(TcpClient tcpClient, CancellationTokenSource cancellationTokenSource)
            {
                _tcpClient = tcpClient;
                _cancellationTokenSource = cancellationTokenSource;
                _ns = tcpClient.GetStream();
            }

            public override async Task<byte[]> ReceiveMessage()
            {
                if(_tcpClient.ReceiveBufferSize == 0) return new byte[0];
                var result = new byte[_tcpClient.ReceiveBufferSize];
                await _ns.ReadAsync(result, 0, _tcpClient.ReceiveBufferSize, _cancellationTokenSource.Token);
                if (result.All(r => r == 0)) return new byte[0];
                return result;
            }

            public override async Task SendMessage(byte[] payload)
            {
                await _ns.WriteAsync(payload, 0, payload.Length, _cancellationTokenSource.Token);
            }
        }
    }
}
