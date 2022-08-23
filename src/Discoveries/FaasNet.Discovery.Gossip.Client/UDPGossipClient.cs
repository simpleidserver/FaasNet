using FaasNet.Common.Extensions;
using FaasNet.Discovery.Gossip.Client.Messages;
using FaasNet.Peer.Client;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.Discovery.Gossip.Client
{
    public class UDPGossipClient : BasePeerClient
    {
        private readonly UdpClient _udpClient;

        public UDPGossipClient(IPEndPoint target) : base(target)
        {
            _udpClient = new UdpClient();
        }

        public UDPGossipClient(string url, int port) : base(url, port)
        {
            _udpClient = new UdpClient();
        }

        public async Task Sync(ICollection<PeerInfo> peerInfos, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var package = GossipPackageRequestBuilder.Sync(peerInfos);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), Target).WithCancellation(cancellationToken, timeoutMS);
            await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
        }

        public async Task<ICollection<PeerInfo>> Get(string partitionKey, CancellationToken cancellationToken = default(CancellationToken), int timeoutMS = 500)
        {
            var writeCtx = new WriteBufferContext();
            var package = GossipPackageRequestBuilder.Get(partitionKey);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await _udpClient.SendAsync(payload, payload.Count(), Target).WithCancellation(cancellationToken, timeoutMS);
            var resultPayload = await _udpClient.ReceiveAsync().WithCancellation(cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var packageResult = GossipPackage.Deserialize(readCtx, true);
            return (packageResult as GossipGetResultPackage).PeerInfos;

        }

        public override void Dispose()
        {
            if (_udpClient != null)
            {
                _udpClient.Close();
                _udpClient.Dispose();
            }
        }
    }
}
