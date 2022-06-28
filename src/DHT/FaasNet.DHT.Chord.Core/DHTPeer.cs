using FaasNet.DHT.Chord.Client;
using FaasNet.DHT.Chord.Client.Messages;
using FaasNet.DHT.Chord.Core.Handlers;
using FaasNet.DHT.Chord.Core.Stores;
using FaasNet.DHT.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FaasNet.DHT.Chord.Core
{
    public interface IDHTPeer
    {
        void Start(string url, int port, CancellationToken token);
        bool IsRunning { get; }
        IDHTPeerInfoStore PeerInfoStore { get; }
        void Stop();
    }

    public class DHTPeer : IDHTPeer
    {
        private readonly DHTOptions _options;
        private readonly ILogger<DHTPeer> _logger;
        private readonly IEnumerable<IRequestHandler> _requestHandlers;
        private readonly IDHTPeerInfoStore _peerInfoStore;
        private CancellationTokenSource _cancellationTokenSource;
        private Socket _server;
        private static ManualResetEvent _lock = new ManualResetEvent(false);
        private System.Timers.Timer _stabilizeTimer;
        private System.Timers.Timer _fixFingersTimer;

        public DHTPeer(IOptions<DHTOptions> options, ILogger<DHTPeer> logger, IEnumerable<IRequestHandler> requestHandlers, IDHTPeerInfoStore peerInfoStore)
        {
            _options = options.Value;
            _logger = logger;
            _requestHandlers = requestHandlers;
            _peerInfoStore = peerInfoStore;
        }

        public bool IsRunning { get; private set; }
        public IDHTPeerInfoStore PeerInfoStore => _peerInfoStore;

        public void Start(string url = Constants.DefaultUrl, int port = Constants.DefaultPort, CancellationToken token = default(CancellationToken))
        {
            if (IsRunning) throw new InvalidOperationException("The peer is already running");
            var peerInfo = new DHTPeerInfo(url, port);
            _peerInfoStore.Update(peerInfo);
            var localEndpoint = new IPEndPoint(DnsHelper.ResolveIPV4(url), port);
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _server.Bind(localEndpoint);
            _server.Listen();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            Task.Run(() => Handle(_server));
            StartStabilizeTimer();
            StartFixFingers();
            IsRunning = true;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _stabilizeTimer.Stop();
            _fixFingersTimer.Stop();
        }

        private void StartStabilizeTimer()
        {
            _stabilizeTimer = new System.Timers.Timer(_options.StabilizeTimerMS);
            _stabilizeTimer.Elapsed += async (o, e) => await Stabilize(o, e);
            _stabilizeTimer.AutoReset = false;
            _stabilizeTimer.Start();
        }

        private void StartFixFingers()
        {
            _fixFingersTimer = new System.Timers.Timer(_options.FixFingersTimerMS);
            _fixFingersTimer.Elapsed += async (o, e) => await FixFingers(o, e);
            _fixFingersTimer.AutoReset = false;
            _fixFingersTimer.Start();
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

        private async Task Stabilize(object source, ElapsedEventArgs e)
        {
            var peerInfo = _peerInfoStore.Get();
            if (peerInfo.SuccessorPeer == null)
            {
                StartStabilizeTimer();
                return;
            }

            using (var chordClient = new ChordClient(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
            {
                var predecessor = chordClient.FindPredecessor();
                if(predecessor.HasPredecessor)
                {
                    if (IntervalHelper.CheckInterval(peerInfo.Peer.Id, predecessor.Id, peerInfo.SuccessorPeer.Id, peerInfo.DimensionFingerTable))
                    {
                        peerInfo.SuccessorPeer = new PeerInfo { Id = predecessor.Id, Port = predecessor.Port, Url = predecessor.Url };
                        _peerInfoStore.Update(peerInfo);
                    }
                }
            }

            using (var secondClient = new ChordClient(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
            {
                secondClient.Notify(peerInfo.Peer.Url, peerInfo.Peer.Port, peerInfo.Peer.Id);
            }

            StartStabilizeTimer();
        }

        private async Task FixFingers(object source, ElapsedEventArgs e)
        {
            var peerInfo = _peerInfoStore.Get();
            if(peerInfo.SuccessorPeer == null)
            {
                StartFixFingers();
                return;
            }

            var fingers = new List<FingerTableRecord>();
            var start = peerInfo.Peer.Id;
            var max = Math.Pow(2, peerInfo.DimensionFingerTable);
            using (var chordClient = new ChordClient(peerInfo.SuccessorPeer.Url, peerInfo.SuccessorPeer.Port))
            {
                for (int i = 1; i <= peerInfo.DimensionFingerTable; i++)
                {
                    var newId = peerInfo.Peer.Id + (long)Math.Pow(2, i - 1);
                    if (newId >= max) break;
                    var successor = chordClient.FindSuccessor(newId);
                    fingers.Add(new FingerTableRecord
                    {
                        Start = start,
                        End = newId,
                        Peer = new PeerInfo { Id = successor.Id, Port = successor.Port, Url = successor.Url }
                    });
                    start = newId;
                }
            }

            peerInfo.Fingers = fingers;
            _peerInfoStore.Update(peerInfo);
            StartFixFingers();
        }
    }
}
