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

        public async Task<IEventMeshClientPubSession> CreatePubSession(string vpn, string clientId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var pubSession = await CreateSession(vpn, clientId, UserAgentPurpose.PUB, cancellationToken);
            return new EventMeshClientPubSession(pubSession, _ipAddr, _port);
        }

        public async Task<IEventMeshClientSubSession> CreateSubSession(string vpn, string clientId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var subSession = await CreateSession(vpn, clientId, UserAgentPurpose.SUB, cancellationToken);
            return new EventMeshClientSubSession(subSession, _ipAddr, _port);
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

        internal static void EnsureSuccessStatus(Package packageRequest, Package packageResponse)
        {
            if (packageResponse.Header.Status != HeaderStatus.SUCCESS)
            {
                throw new RuntimeClientResponseException(packageResponse.Header.Status, packageResponse.Header.Error);
            }

            if (packageRequest.Header.Seq != packageRequest.Header.Seq)
            {
                throw new RuntimeClientResponseException(HeaderStatus.FAIL, Errors.INVALID_SEQ, "the seq in the request doesn't match the seq in the response");
            }
        }

        private async Task<HelloResponse> CreateSession(string vpn, string clientId, UserAgentPurpose purpose, CancellationToken cancellationToken = default(CancellationToken))
        {
            var userAgent = new UserAgent { ClientId = clientId, Vpn = vpn, Purpose = purpose };
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
        Task Publish(string topicName, CloudEvent cloudEvent, CancellationToken cancellationToken = default(CancellationToken));
    }

    public interface IEventMeshClientSubSession
    {
        Task<SubscriptionResult> PersistedSubscribe(string topicFilter, string groupId, Action<CloudEvent> callback, CancellationToken cancellationToken);
        SubscriptionResult DirectSubscribe(string topicFilter, Action<CloudEvent> callback, CancellationToken cancellationToken);
    }

    public class SubscriptionResult
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        internal SubscriptionResult(CancellationTokenSource cancellationTokenSource)
        {
            _cancellationTokenSource = cancellationTokenSource;
        }

        public void Close()
        {
            _cancellationTokenSource.Cancel();
        }
    }

    internal class EventMeshClientPubSession : IEventMeshClientPubSession
    {
        private readonly HelloResponse _session;
        private readonly IPAddress _ipAddr;
        private readonly int _port;

        public EventMeshClientPubSession(HelloResponse session, IPAddress ipAddr, int port)
        {
            _session = session;
            _ipAddr = ipAddr;
            _port = port;
        }

        public Task Publish(string topicName, object obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            var cloudEvt = new CloudEvent
            {
                Id = Guid.NewGuid().ToString(),
                Subject = topicName,
                Source = new Uri("http://localhost"),
                Type = topicName,
                DataContentType = "application/json",
                Data = JsonSerializer.Serialize(obj),
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
    }

    internal class EventMeshClientSubSession : IEventMeshClientSubSession
    {
        private readonly HelloResponse _session;
        private readonly IPAddress _ipAddr;
        private readonly int _port;
        private int _offsetEvt;

        public EventMeshClientSubSession(HelloResponse session, IPAddress ipAddr, int port)
        {
            _session = session;
            _ipAddr = ipAddr;
            _port = port;
        }

        public async Task<SubscriptionResult> PersistedSubscribe(string topicFilter, string groupId, Action<CloudEvent> callback, CancellationToken cancellationToken)
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var result = new SubscriptionResult(cancellationTokenSource);
            await PersistedSubscribe(topicFilter, groupId, cancellationToken);
#pragma warning disable 4014
            Task.Run(async () => await HandlePersistedSubscribe(callback, groupId, cancellationTokenSource.Token));
#pragma warning restore 4014
            return result;
        }

        public SubscriptionResult DirectSubscribe(string topicName, Action<CloudEvent> callback, CancellationToken cancellationToken)
        {
            _offsetEvt = 1;
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var result = new SubscriptionResult(cancellationTokenSource);
#pragma warning disable 4014
            Task.Run(async () => await HandleDirectSubscribe(callback, topicName, cancellationTokenSource.Token));
#pragma warning restore 4014
            return result;
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
