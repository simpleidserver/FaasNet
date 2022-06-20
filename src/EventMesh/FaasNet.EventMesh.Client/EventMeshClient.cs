using CloudNative.CloudEvents;
using FaasNet.EventMesh.Client.Exceptions;
using FaasNet.EventMesh.Client.Messages;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Client
{
    public class EventMeshClient
    {
        private readonly IPAddress _ipAddr;
        private readonly int _port;

        public EventMeshClient(string url = Constants.DefaultUrl, int port = Constants.DefaultPort)
        {
            _ipAddr = IPAddressHelper.ResolveIPAddress(url);
            _port = port;
        }

        public async Task Ping(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.HeartBeat();
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
            }
        }

        public async Task<IEnumerable<string>> GetAllVpns(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.GetAllVpns();
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
                var result = packageResult as GetAllVpnResponse;
                return result.Vpns;
            }
        }

        public async Task<IEventMeshClientPubSession> CreatePubSession(string vpn, string clientId, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var pubSession = await CreateSession(vpn, clientId, UserAgentPurpose.PUB, expirationTime, false, cancellationToken);
            return new EventMeshClientPubSession(pubSession, clientId, _ipAddr, _port);
        }

        public async Task<IEventMeshClientPubSession> CreatePubSession(string vpn, string clientId, TimeSpan? expirationTime = null, bool isInfinite = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var pubSession = await CreateSession(vpn, clientId, UserAgentPurpose.PUB, expirationTime, isInfinite, cancellationToken);
            return new EventMeshClientPubSession(pubSession, clientId, _ipAddr, _port);
        }

        public async Task<IEventMeshClientSubSession> CreateSubSession(string vpn, string clientId, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var subSession = await CreateSession(vpn, clientId, UserAgentPurpose.SUB, expirationTime, false, cancellationToken);
            return new EventMeshClientSubSession(subSession, clientId, _ipAddr, _port);
        }

        public async Task AddVpn(string vpn, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.AddVPN(vpn);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
            }
        }

        public async Task AddClient(string vpn, string clientId, List<UserAgentPurpose> purposes, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.AddClient(vpn, clientId, purposes);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
            }
        }

        public async Task<IEnumerable<BridgeServerResponse>> GetAllBridges(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.GetAllBridge();
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
                return (packageResult as GetAllBridgeResponse).Servers;
            }
        }

        public async Task AddBridge(string vpn, string url, int port, string targetVpn, string targetClientId, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.AddBridge(vpn, url, port, targetVpn, targetClientId);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
            }
        }

        public async Task Disconnect(string clientId, string sessionId, CancellationToken cancellationToken = default(CancellationToken))
        {
            using(var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.Disconnect(clientId, sessionId);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
            }
        }

        public async Task<ICollection<PluginResponse>> GetAllPlugins(CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.GetAllPlugins();
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
                return (packageResult as GetAllPluginsResponse).Plugins;
            }
        }

        public async Task EnablePlugin(string pluginName, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.EnablePlugin(pluginName);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
            }
        }

        public async Task DisablePlugin(string pluginName, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.DisablePlugin(pluginName);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
            }
        }

        public async Task<ICollection<PluginConfigurationRecordResponse>> GetPluginConfiguration(string pluginName, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.GetPluginConfiguration(pluginName);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
                return (packageResult as GetPluginConfigurationResponse).Records;
            }
        }

        public async Task UpdatePluginConfiguration(string pluginName, string propertyKey, string propertyValue, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.UpdatePluginConfiguration(pluginName, propertyKey, propertyValue);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
            }
        }

        internal static void EnsureSuccessStatus(Package packageRequest, Package packageResponse)
        {
            if (packageResponse.Header.Status != HeaderStatus.SUCCESS) throw new RuntimeClientResponseException(packageResponse.Header.Status, packageResponse.Header.Error);
            if (packageRequest.Header.Seq != packageRequest.Header.Seq) throw new RuntimeClientResponseException(HeaderStatus.FAIL, Errors.INVALID_SEQ, "the seq in the request doesn't match the seq in the response");
        }

        private async Task<HelloResponse> CreateSession(string vpn, string clientId, UserAgentPurpose purpose, TimeSpan? expirationTime = null, bool isInfinite = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userAgent = new UserAgent { ClientId = clientId, Vpn = vpn, Purpose = purpose, Expiration = expirationTime, IsSessionInfinite = isInfinite };
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.Hello(userAgent);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EnsureSuccessStatus(package, packageResult);
                return packageResult as HelloResponse;
            }
        }
    }

    public interface IEventMeshClientPubSession
    {
        Task Publish(string topicName, object obj, CancellationToken cancellationToken = default(CancellationToken));
        Task Publish(string topicName, string str, CancellationToken cancellationToken = default(CancellationToken));
        Task Publish(string topicName, CloudEvent cloudEvent, CancellationToken cancellationToken = default(CancellationToken));
        Task Disconnect(CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IEventMeshClientSubSession
    {
        string SessionId { get; }
        Task<SubscriptionResult> PersistedSubscribe(string topicFilter, string groupId, Action<CloudEvent> callback, CancellationToken cancellationToken);
        SubscriptionResult DirectSubscribe(string topicFilter, Action<CloudEvent> callback, CancellationToken cancellationToken);
        Task Disconnect(CancellationToken cancellationToken = default(CancellationToken));
    }

    public class SubscriptionResult
    {
        private readonly Func<CancellationToken, Task> _disconnectCallback;

        internal SubscriptionResult(Func<CancellationToken, Task> disconnectCallback)
        {
            _disconnectCallback = disconnectCallback;
        }

        public async void Close()
        {
            await _disconnectCallback(CancellationToken.None);
        }
    }

    internal class EventMeshClientPubSession : IEventMeshClientPubSession
    {
        private readonly HelloResponse _session;
        private readonly string _clientId;
        private readonly IPAddress _ipAddr;
        private readonly int _port;

        public EventMeshClientPubSession(HelloResponse session, string clientId, IPAddress ipAddr, int port)
        {
            _session = session;
            _clientId = clientId;
            _ipAddr = ipAddr;
            _port = port;
        }

        public Task Publish(string topicName, object obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Publish(topicName, JsonSerializer.Serialize(obj), cancellationToken);
        }

        public Task Publish(string topicName, string str, CancellationToken cancellationToken = default(CancellationToken))
        {
            var cloudEvt = new CloudEvent
            {
                Id = Guid.NewGuid().ToString(),
                Subject = topicName,
                Source = new Uri("http://localhost"),
                Type = topicName,
                DataContentType = "application/json",
                Data = str,
                Time = DateTimeOffset.UtcNow
            };
            return Publish(topicName, cloudEvt, cancellationToken);
        }

        public async Task Publish(string topicName, CloudEvent cloudEvent, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.PublishMessage(_session.SessionId, topicName, cloudEvent);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port));
                var resultPayload = await udpClient.ReceiveAsync();
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EventMeshClient.EnsureSuccessStatus(package, packageResult);
            }
        }

        public async Task Disconnect(CancellationToken cancellationToken = default)
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.Disconnect(_clientId, _session.SessionId);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port));
                var resultPayload = await udpClient.ReceiveAsync();
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EventMeshClient.EnsureSuccessStatus(package, packageResult);
            }
        }
    }

    internal class EventMeshClientSubSession : IEventMeshClientSubSession
    {
        private readonly HelloResponse _session;
        private readonly string _clientId;
        private readonly IPAddress _ipAddr;
        private readonly int _port;
        private int _offsetEvt;
        private CancellationTokenSource _activeSubscriptionCancellationTokenSource = null;

        public EventMeshClientSubSession(HelloResponse session, string clientId, IPAddress ipAddr, int port)
        {
            _session = session;
            _clientId = clientId;
            _ipAddr = ipAddr;
            _port = port;
        }

        public string SessionId => _session.SessionId;

        public async Task<SubscriptionResult> PersistedSubscribe(string topicFilter, string groupId, Action<CloudEvent> callback, CancellationToken cancellationToken)
        {
            if (_activeSubscriptionCancellationTokenSource != null) throw new InvalidOperationException("There is already an active subscription");
            _activeSubscriptionCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var activeSubscription = new SubscriptionResult(Disconnect);
            await PersistedSubscribe(topicFilter, groupId, cancellationToken);
#pragma warning disable 4014
            Task.Run(async () => await HandlePersistedSubscribe(callback, groupId, _activeSubscriptionCancellationTokenSource.Token));
#pragma warning restore 4014
            return activeSubscription;
        }

        public SubscriptionResult DirectSubscribe(string topicName, Action<CloudEvent> callback, CancellationToken cancellationToken)
        {
            if (_activeSubscriptionCancellationTokenSource != null) throw new InvalidOperationException("There is already an active subscription");
            _offsetEvt = 1;
            _activeSubscriptionCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var activeSubscription = new SubscriptionResult(Disconnect);
#pragma warning disable 4014
            Task.Run(async () => await HandleDirectSubscribe(callback, topicName, _activeSubscriptionCancellationTokenSource.Token));
#pragma warning restore 4014
            return activeSubscription;
        }

        public async Task Disconnect(CancellationToken cancellationToken = default)
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.Disconnect(_clientId, _session.SessionId);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port));
                var resultPayload = await udpClient.ReceiveAsync();
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EventMeshClient.EnsureSuccessStatus(package, packageResult);
            }

            if (_activeSubscriptionCancellationTokenSource != null) _activeSubscriptionCancellationTokenSource.Cancel();
        }

        private async Task PersistedSubscribe(string topicFilter, string groupId, CancellationToken cancellationToken)
        {
            using (var udpClient = new UdpClient())
            {
                var writeCtx = new WriteBufferContext();
                var package = PackageRequestBuilder.Subscribe(groupId, new List<SubscriptionItem>
                {
                    new SubscriptionItem
                    {
                        Topic = topicFilter
                    }
                }, _session.SessionId);
                package.Serialize(writeCtx);
                var payload = writeCtx.Buffer.ToArray();
                await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                var readCtx = new ReadBufferContext(resultPayload.Buffer);
                var packageResult = Package.Deserialize(readCtx);
                EventMeshClient.EnsureSuccessStatus(package, packageResult);
            }
        }

        private async Task HandlePersistedSubscribe(Action<CloudEvent> callback, string groupId, CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    using (var udpClient = new UdpClient())
                    {
                        var writeCtx = new WriteBufferContext();
                        var package = PackageRequestBuilder.ReadNextMessage(_session.SessionId, groupId);
                        package.Serialize(writeCtx);
                        var payload = writeCtx.Buffer.ToArray();
                        await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                        var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                        var readCtx = new ReadBufferContext(resultPayload.Buffer);
                        var packageResult = Package.Deserialize(readCtx);
                        EventMeshClient.EnsureSuccessStatus(package, packageResult);
                        var result = packageResult as ReadNextMessageResponse;
                        if (result.ContainsMessage)
                        {
                            callback(result.CloudEvt);
                        }
                    }

                    Thread.Sleep(200);
                }
            }
            catch { }
        }

        private async Task HandleDirectSubscribe(Action<CloudEvent> callback, string topicName, CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    using (var udpClient = new UdpClient())
                    {
                        var writeCtx = new WriteBufferContext();
                        var package = PackageRequestBuilder.ReadTopicMessage(topicName, _offsetEvt);
                        package.Serialize(writeCtx);
                        var payload = writeCtx.Buffer.ToArray();
                        await udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddr, _port)).WithCancellation(cancellationToken);
                        var resultPayload = await udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                        var readCtx = new ReadBufferContext(resultPayload.Buffer);
                        var packageResult = Package.Deserialize(readCtx);
                        EventMeshClient.EnsureSuccessStatus(package, packageResult);
                        var result = packageResult as ReadMessageTopicResponse;
                        if (result.ContainsMessage)
                        {
                            callback(result.Value);
                            _offsetEvt++;
                        }
                    }

                    Thread.Sleep(200);
                }
            }
            catch { }
        }
    }
}
