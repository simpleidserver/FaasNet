using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Core
{
    public interface IDHTPeer
    {
        bool IsRunning { get; }
    }

    public class DHTPeer : IDHTPeer
    {
        private readonly ILogger<DHTPeer> _logger;
        private CancellationTokenSource _cancellationTokenSource;
        private UdpClient _server;

        public DHTPeer(ILogger<DHTPeer> logger)
        {
            _logger = logger;
        }

        public bool IsRunning { get; private set; }

        public void Start(int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsRunning) throw new InvalidOperationException("The peer is already started");
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var localEdp = new IPEndPoint(IPAddress.Any, port);
            _server = new UdpClient(localEdp);
#pragma warning disable 4014
            Task.Run(async () => await InternalRun(), cancellationToken);
#pragma warning restore 4014
        }

        public void Stop()
        {
            if (!IsRunning) throw new InvalidOperationException("The peer is not started");
            _cancellationTokenSource?.Cancel();
            _server.Close();
            IsRunning = false;
        }

        private async Task InternalRun()
        {
            try
            {
                while (true)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    var receiveResult = await _server.ReceiveAsync();
                    await HandlePackage(receiveResult);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task HandlePackage(UdpReceiveResult result)
        {

        }
    }
}
