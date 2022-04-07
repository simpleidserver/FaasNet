using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.Exceptions;
using FaasNet.EventMesh.Client.Extensions;
using FaasNet.EventMesh.Client.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Client
{
    public class RuntimeClient
    {
        private UdpClient _udpClient;
        private readonly IPAddress _clientIPAddress;
        private readonly int _clientPort;
        private readonly IPAddress _ipAddr;
        private readonly int _port;

        #region Constructors

        public RuntimeClient(UdpClient udpClient, int port = Constants.DefaultPort)
        {
            _udpClient = udpClient;
            _port = port;
            var clientEdp = _udpClient.Client.LocalEndPoint as IPEndPoint;
            _clientIPAddress = clientEdp.Address;
            _clientPort = clientEdp.Port;
        }

        public RuntimeClient(UdpClient udpClient, IPAddress ipAddress, int port = Constants.DefaultPort) : this(udpClient, port)
        {
            _ipAddr = ipAddress;
        }

        public RuntimeClient(UdpClient udpClient, string url = Constants.DefaultUrl, int port = Constants.DefaultPort) : this(udpClient, port)
        {
            _ipAddr = ResolveIPAddress(url);
        }

        public RuntimeClient(string url = Constants.DefaultUrl, int port = Constants.DefaultPort) : this(new UdpClient(new IPEndPoint(IPAddress.Any, 0)), url, port) { }

        #endregion

        #region Properties

        public UdpClient UdpClient
        {
            get
            {
                return _udpClient;
            }
        }

        #endregion

        #region Actions

        public async Task<Package> HeartBeat(CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.HeartBeat();
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
            var resultPayload = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var packageResult = Package.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult;
        }

        public async Task<GetAllVpnResponse> GetAllVpns(CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.GetAllVpns();
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
            var resultPayload = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var packageResult = Package.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult as GetAllVpnResponse;
        }

        public Task<HelloResponse> Hello(UserAgent userAgent, CancellationToken cancellationToken = default(CancellationToken))
        {
            return HandleException(userAgent.ClientId, string.Empty, async () =>
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.Hello(userAgent);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
                return packageResult as HelloResponse;
            });
        }

        public async Task<SubscriptionResult> Subscribe(string clientId, string sessionId, ICollection<SubscriptionItem> subscriptionItems, Action<AsyncMessageToClient> callback = null, string seq = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await HandleException(clientId, sessionId, async() =>
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.Subscribe(clientId, subscriptionItems, sessionId, seq);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var result = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, result);
                return result;
            });
            MessageCallbackListener listener = null;
            if (callback != null)
            {
                listener = new MessageCallbackListener(clientId, _udpClient, callback, _ipAddr, _port, sessionId, () =>
                {
                    _udpClient = BuildNewUdpClient();
                });
                listener.Listen();
            }

            return new SubscriptionResult(result, listener);
        }

        public void Close()
        {
            if (_udpClient != null)
            {
                _udpClient.Close();
            }
        }

        public async Task<Package> AddBridge(string vpn, string urn, int port, string targetVpn, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.AddBridge(vpn, urn, port, targetVpn);
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
            var resultPayload = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var packageResult = Package.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult;
        }

        public async Task<Package> Disconnect(string clientId, string sessionId, bool ignoreException = false)
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.Disconnect(clientId, sessionId);
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port));
            var resultPayload = await _udpClient.ReceiveAsync();
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var packageResult = Package.Deserialize(readCtx);
            if (!ignoreException)
            {
                EnsureSuccessStatus(package, packageResult);
            }

            return packageResult;
        }

        public async Task<Package> TransferMessageToServerFromServer(string clientId, string brokerName, string topic, int nbEventsConsumed, ICollection<AsyncMessageBridgeServer> bridgeServers, string sessionId, string seq = null)
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.AsyncMessageAckToServer(clientId, brokerName, topic, nbEventsConsumed, bridgeServers, sessionId, seq);
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port));
            var resultPayload = await _udpClient.ReceiveAsync();
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var packageResult = Package.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult;
        }

        public async Task TransferMessageToServerFromClient(string clientId, string brokerName, string topic, int nbEventsConsumed, ICollection<AsyncMessageBridgeServer> bridgeServers, string sessionId, string seq = null)
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.AsyncMessageAckToServer(clientId, brokerName, topic, nbEventsConsumed, bridgeServers, sessionId, seq, true);
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port));
        }

        public async Task<Package> PublishMessage(string clientId, string sessionId, string topicName, CloudEvent cloudEvent, string urn = null, int port = default(int), string seq = null)
        {
            var writeCtx = new WriteBufferContext();
            var package = PackageRequestBuilder.PublishMessage(clientId, sessionId, topicName, cloudEvent, urn, port, seq);
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port));
            var resultPayload = await _udpClient.ReceiveAsync();
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var packageResult = Package.Deserialize(readCtx);
            EnsureSuccessStatus(package, packageResult);
            return packageResult;
        }

        #endregion

        public static IPAddress ResolveIPAddress(string url)
        {
            try
            {
                var hostEntry = Dns.GetHostEntry(url);
                return hostEntry.AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);
            }
            catch
            {
                throw new RuntimeClientException($"The url : '{url}' is not correct");
            }
        }

        private UdpClient BuildNewUdpClient()
        {
            return new UdpClient(new IPEndPoint(_clientIPAddress, _clientPort));
        }

        private static void EnsureSuccessStatus(Package packageRequest, Package packageResponse)
        {
            if (packageResponse.Header.Status != HeaderStatus.SUCCESS)
            {
                throw new RuntimeClientResponseException(packageResponse.Header.Status, packageResponse.Header.Error);
            }

            if(packageRequest.Header.Seq != packageRequest.Header.Seq)
            {
                throw new RuntimeClientResponseException(HeaderStatus.FAIL, Errors.INVALID_SEQ, "the seq in the request doesn't match the seq in the response");
            }
        }

        private async Task<T> HandleException<T>(string clientId, string sessionId, Func<Task<T>> callback) where T : Package
        {
            try
            {
                return await callback();
            }
            catch(Exception ex)
            {
                if (ex is ObjectDisposedException || ex is OperationCanceledException)
                {
                    using (var udpClient = BuildNewUdpClient())
                    {
                        var runtimeClient = new RuntimeClient(udpClient, _ipAddr, _port);
                        await runtimeClient.Disconnect(clientId, sessionId);
                    }

                    throw new RuntimeClientSessionClosedException(ex.ToString());
                }

                throw;
            }
        }
    }

    public class MessageCallbackListener
    {
        private readonly string _clientId;
        private readonly UdpClient _udpClient;
        private readonly Action<AsyncMessageToClient> _callback;
        private readonly IPAddress _ipAddr;
        private readonly int _port;
        private readonly string _sessionId;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Action _resetConnection;

        public MessageCallbackListener(
            string clientId,
            UdpClient udpClient, 
            Action<AsyncMessageToClient> callback,
            IPAddress ipAddr,
            int port,
            string sessionId,
            Action resetConnection)
        {
            _clientId = clientId;
            _udpClient = udpClient;
            _callback = callback;
            _ipAddr = ipAddr;
            _port = port;
            _sessionId = sessionId;
            _resetConnection = resetConnection;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Listen()
        {
            Task.Run(async () => await Run(_cancellationTokenSource.Token));
        }

        public void Close()
        {
            _cancellationTokenSource.Cancel();
            _udpClient.Close();
            _resetConnection();
        }

        private async Task Run(CancellationToken cancellationToken)
        {
            while(true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                UdpReceiveResult receiveResult;
                try
                {
                    receiveResult = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                }
                catch (Exception)
                {
                    return;
                }

                var buffer = receiveResult.Buffer;
                var package = Package.Deserialize(new ReadBufferContext(buffer));
                if (package.Header.Command == Commands.ASYNC_MESSAGE_TO_CLIENT)
                {
                    var asyncMessage = package as AsyncMessageToClient;
                    var runtimeClient = new RuntimeClient(_udpClient, _ipAddr, _port);
                    await runtimeClient.TransferMessageToServerFromClient(_clientId, asyncMessage.BrokerName, asyncMessage.Topic, asyncMessage.CloudEvents.Count(), asyncMessage.BridgeServers, _sessionId);
                    _callback(asyncMessage);
                }
            }
        }
    }

    public class SubscriptionResult
    {
        public SubscriptionResult(Package package, MessageCallbackListener listener)
        {
            Package = package;
            Listener = listener;
        }

        public Package Package { get; set; }
        public MessageCallbackListener Listener { get; set; }

        public void Stop()
        {
            Listener.Close();
        }
    }
}
