using FaasNet.Common.Extensions;
using FaasNet.Common.Helpers;
using FaasNet.Discovery.Gossip.Client.Messages;
using FaasNet.Peer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Discovery.Gossip.Client
{
    public class UDPGossipClient : IDisposable
    {
        private readonly IPAddress _ipAddress;
        private readonly int _port;
        private readonly UdpClient _udpClient;

        public UDPGossipClient(string url, int port)
        {
            _ipAddress = DnsHelper.ResolveIPV4(url);
            _port = port;
            _udpClient = new UdpClient();
        }

        public async Task Sync(ICollection<PeerInfo> peerInfos, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var package = GossipPackageRequestBuilder.Sync(peerInfos);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddress, _port)).WithCancellation(cancellationToken, timeoutMS);
            await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
        }

        public async Task<ICollection<PeerInfo>> Get(CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var package = GossipPackageRequestBuilder.Get();
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), new IPEndPoint(_ipAddress, _port)).WithCancellation(cancellationToken, timeoutMS);
            var resultPayload = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var packageResult = GossipPackage.Deserialize(readCtx, true);
            return (packageResult as GossipGetResultPackage).PeerInfos;

        }

        public void Dispose()
        {
            if (_udpClient != null)
            {
                _udpClient.Close();
                _udpClient.Dispose();
            }
        }
    }
}
