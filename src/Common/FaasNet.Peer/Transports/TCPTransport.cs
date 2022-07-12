using FaasNet.Common.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
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
        private readonly ILogger<TCPTransport> _logger;
        private ManualResetEvent _sessionLock = new ManualResetEvent(false);
        private ManualResetEvent _messageReceivedLock = new ManualResetEvent(false);
        private SemaphoreSlim _readLock = new SemaphoreSlim(1);
        private Socket _tcpServer;
        private CancellationTokenSource _cancellationTokenSource;
        private TCPSession _activeTCPSession;

        public TCPTransport(IOptions<PeerOptions> options, ILogger<TCPTransport> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public void Start(CancellationToken cancellationToken = default(CancellationToken))
        {
            var localEndpoint = new IPEndPoint(DnsHelper.ResolveIPV4(_options.Url), _options.Port);
            _sessionLock = new ManualResetEvent(false);
            _readLock = new SemaphoreSlim(1);
            _tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _tcpServer.Bind(localEndpoint);
            _tcpServer.Listen();
            Task.Run(() => Handle());
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        public void Stop()
        {
            _tcpServer.Close();
        }

        public Task<MessageResult> ReceiveMessage()
        {
            _messageReceivedLock.WaitOne();
            var result = new MessageResult(_activeTCPSession.ReceivedMessage, _activeTCPSession.Send);
            _messageReceivedLock.Reset();
            return Task.FromResult(result);
        }

        private void Handle()
        {
            try
            {
                while(true)
                {
                    _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    _sessionLock.Reset();
                    _tcpServer.BeginAccept(new AsyncCallback(AcceptCallback), _tcpServer);
                    _sessionLock.WaitOne();

                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            _sessionLock.Set();
            try
            {
                var listener = (Socket)ar.AsyncState;
                var handler = listener.EndAccept(ar);
                var state = new SessionStateObject(handler);
                _activeTCPSession = new TCPSession(state, _messageReceivedLock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private class TCPSession
        {
            private readonly SessionStateObject _sessionStateObject;
            private byte[] _receivedMessage;
            private ManualResetEvent _messageReceivedLock;

            public TCPSession(SessionStateObject sessionStateObject, ManualResetEvent manualResetEvent)
            {
                _sessionStateObject = sessionStateObject;
                _sessionStateObject.SessionSocket.BeginReceive(_sessionStateObject.Buffer, 0, SessionStateObject.BufferSize, 0, new AsyncCallback(ReadCallback), _sessionStateObject);
                _messageReceivedLock = manualResetEvent;
            }

            public byte[] ReceivedMessage => _receivedMessage;

            public Task Send(byte[] payload)
            {
                _sessionStateObject.SessionSocket.BeginSend(payload, 0, payload.Count(), 0, new AsyncCallback(SendCallback), _sessionStateObject);
                return Task.CompletedTask;
            }

            private void ReadCallback(IAsyncResult ar)
            {
                var state = (SessionStateObject)ar.AsyncState;
                var handler = state.SessionSocket;
                var nbBytes = handler.EndReceive(ar);
                var buffer = state.Buffer.Take(nbBytes).ToArray();
                _receivedMessage = buffer;
                _messageReceivedLock.Set();
            }

            private void SendCallback(IAsyncResult ar)
            {
                var state = (SessionStateObject)ar.AsyncState;
                var newState = new SessionStateObject(state.SessionSocket);
                state.SessionSocket.BeginReceive(newState.Buffer, 0, SessionStateObject.BufferSize, 0, new AsyncCallback(ReadCallback), newState);
            }
        }
    }
}
