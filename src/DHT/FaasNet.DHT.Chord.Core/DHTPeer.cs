using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core
{
    public class DHTPeer
    {
        private readonly DHTOptions _options;
        private CancellationTokenSource _cancellationTokenSource;
        private TcpListener _server;

        public DHTPeer(IOptions<DHTOptions> options)
        {
            _options = options.Value;
            PeerInfo = new DHTPeerInfo(_options.Url, _options.Port, _options.M);
        }

        public DHTPeerInfo PeerInfo { get; private set; }
        public bool IsRunning { get; private set; }

        public async Task Start(CancellationToken token)
        {
            if (IsRunning) throw new InvalidOperationException("The peer is already running");
            _server = new TcpListener(IPAddress.Parse(_options.Url), _options.Port);
            _server.Start();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
#pragma warning disable 4014
            Task.Run(async () => await InternalRun(), token);
#pragma warning restore 4014
            IsRunning = true;
        }

        public void Stop()
        {
            if (!IsRunning) throw new InvalidOperationException("The peer is not running");
            _server.Stop();
            _cancellationTokenSource.Cancel();
        }

        private async Task InternalRun()
        {
            while(true)
            {
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                // https://github.com/edoardoramalli/Chord/blob/801e00d21fe9f09fd4c705fcf4d7a08b1b7171a8/src/main/java/node/Node.java
                // https://resources.mpi-inf.mpg.de/d5/teaching/ws03_04/p2p-data/11-18-writeup1.pdf
                // https://arxiv.org/pdf/2109.10787.pdf
                // CONTINUE.
            }
        }
    }
}
