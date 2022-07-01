﻿using FaasNet.DHT.Kademlia.Client.Extensions;
using FaasNet.DHT.Kademlia.Client.Messages;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Client
{
    public class KademliaClient : IDisposable
    {
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        private readonly UdpClient _udpClient;

        public KademliaClient(string url, int port)
        {
            _ipAddress = DnsHelper.ResolveIPV4(url);
            _port = port;
            _udpClient = new UdpClient();
        }

        public async Task<FindValueResult> FindValue(long key, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = PackageRequestBuilder.FindValue(key, nonce);
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddress, _port)).WithCancellation(cancellationToken);
            var resultPayload = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var packageResult = BasePackage.Deserialize(readCtx);
            return packageResult as FindValueResult;
        }

        public async Task<FindNodeResult> FindNode(long key, string url, int port, long targetId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = PackageRequestBuilder.FindNode(key, url, port, targetId, nonce);
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddress, _port)).WithCancellation(cancellationToken);
            var resultPayload = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var packageResult = BasePackage.Deserialize(readCtx);
            return packageResult as FindNodeResult;
        }

        public async Task<StoreResult> StoreValue(long key, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = PackageRequestBuilder.StoreValue(key, value, nonce);
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddress, _port)).WithCancellation(cancellationToken);
            var resultPayload = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var packageResult = BasePackage.Deserialize(readCtx);
            return packageResult as StoreResult;
        }

        public async Task<JoinResult> Join(long key, string url, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var nonce = Guid.NewGuid().ToString();
            var package = PackageRequestBuilder.Join(key, url, port, nonce);
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddress, _port)).WithCancellation(cancellationToken);
            var resultPayload = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var packageResult = BasePackage.Deserialize(readCtx);
            return packageResult as JoinResult;
        }

        public void Dispose()
        {
            if(_udpClient != null)
            {
                _udpClient.Close();
                _udpClient.Dispose();
            }
        }
    }
}
