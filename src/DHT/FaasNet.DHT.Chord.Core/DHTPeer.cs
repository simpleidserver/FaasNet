using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Handlers;
using FaasNet.DHT.Chord.Core.Stores;
using FaasNet.DHT.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Chord.Core
{
    public interface IDHTPeer
    {
        void Start(DHTOptions options, CancellationToken token);
        void Stop();
    }

    public class DHTPeer : IDHTPeer
    {
        private readonly ILogger<DHTPeer> _logger;
        private readonly IEnumerable<IRequestHandler> _requestHandlers;
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private CancellationTokenSource _cancellationTokenSource;
        private Socket _server;
        private static ManualResetEvent _lock = new ManualResetEvent(false);

        public DHTPeer(ILogger<DHTPeer> logger, IEnumerable<IRequestHandler> requestHandlers, IDHTPeerInfoStore peerInfoStore)
        {
            _logger = logger;
            _requestHandlers = requestHandlers;
            _peerInfoStore = peerInfoStore;
        }

        public bool IsRunning { get; private set; }

        public void Start(DHTOptions options, CancellationToken token)
        {
            if (IsRunning) throw new InvalidOperationException("The peer is already running");
            var peerInfo = new DHTPeerInfo(options.Url, options.Port, options.DimensionFingerTable, options.DimensionSuccessor);
            _peerInfoStore.Update(peerInfo);
            var localEndpoint = new IPEndPoint(DnsHelper.ResolveIPV4(options.Url), options.Port);
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(localEndpoint);
            _server.Listen();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            Task.Run(() => Handle(_server));
            IsRunning = true;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private void Handle(Socket server)
        {
            while(true)
            {
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                _lock.Reset();
                server.BeginAccept(new AsyncCallback(AcceptCallback), server);
                _lock.WaitOne();
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            _lock.Set();
            try
            {
                var listener = (Socket)ar.AsyncState;
                var handler = listener.EndAccept(ar);
                var state = new SessionStateObject(handler);
                handler.BeginReceive(state.Buffer, 0, SessionStateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async void ReadCallback(IAsyncResult ar)
        {
            var state = (SessionStateObject)ar.AsyncState;
            var handler = state.SessionSocket;
            try
            {
                // CONTINUE
                // https://github.com/edoardoramalli/Chord/blob/801e00d21fe9f09fd4c705fcf4d7a08b1b7171a8/src/main/java/node/Node.java
                // https://resources.mpi-inf.mpg.de/d5/teaching/ws03_04/p2p-data/11-18-writeup1.pdf
                // https://arxiv.org/pdf/2109.10787.pdf
                var nbBytes = handler.EndReceive(ar);
                var buffer = state.Buffer.Take(nbBytes).ToArray();
                var readBufferContext = new ReadBufferContext(buffer);
                var request = DHTPackage.Deserialize(readBufferContext);
                _logger.LogInformation("Receive {command}", request.Command.Name);
                var requestHandler = _requestHandlers.First(r => r.Command == request.Command);
                var response = await requestHandler.Handle(request, _cancellationTokenSource.Token);
                var writeBufferContext = new WriteBufferContext();
                response.Serialize(writeBufferContext);
                var result = writeBufferContext.Buffer.ToArray();
                state.SessionSocket.BeginSend(result, 0, result.Count(), 0, new AsyncCallback(SendCallback), state);
                _logger.LogInformation("Response {command}", response.Command.Name);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            var state = (SessionStateObject)ar.AsyncState;
            var newState = new SessionStateObject(state.SessionSocket);
            state.SessionSocket.BeginReceive(newState.Buffer, 0, SessionStateObject.BufferSize, 0, new AsyncCallback(ReadCallback), newState);
        }
    }
}
