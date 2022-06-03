using FaasNet.RaftConsensus.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Client.Messages.Gossip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Client
{
    public class GossipClient : IDisposable
    {
        private readonly IPEndPoint _target;

        public GossipClient(IPEndPoint target)
        {
            _target = target;
            UdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        }

        public GossipClient(string url, int port) : this(new IPEndPoint(IPAddressHelper.ResolveIPAddress(url), port)) { }

        public UdpClient UdpClient { get; private set; }

        public Task Heartbeat(string url, int port, int? timeout, CancellationToken cancellationToken = default(CancellationToken))
        {
            var package = GossipPackageRequestBuilder.Heartbeat(url, port);
            return Send(package, timeout, cancellationToken);
        }

        public Task AddPeer(string termId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var package = GossipPackageRequestBuilder.AddPeer(termId);
            return Send(package, cancellationToken: cancellationToken);
        }

        public Task UpdateNodeState(string entityType, string entityId, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            var package = GossipPackageRequestBuilder.UpdateNodeState(string.Empty, 0, entityType, entityId, value);
            return Send(package, cancellationToken: cancellationToken);
        }

        public async Task<ICollection<ClusterNodeResult>> GetClusterNodes(CancellationToken cancellationToken = default(CancellationToken))
        {
            var package = GossipPackageRequestBuilder.GetClusterNodes();
            var writeCtx = new WriteBufferContext();
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await UdpClient.SendAsync(payload, payload.Count(), _target).WithCancellation(cancellationToken);
            var udpReceivedResult = await UdpClient.ReceiveAsync();
            var readCtx = new ReadBufferContext(udpReceivedResult.Buffer);
            return (GossipPackage.Deserialize(readCtx) as GossipGetClusterNodesResult).ClusterNodes;
        }

        public async Task Send(GossipPackage gossipPackage, int? timeoutMS = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            gossipPackage.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await UdpClient.SendAsync(payload, payload.Count(), _target).WithCancellation(cancellationToken, timeoutMS);
            if (UdpClient.Available == 1) throw new TimeoutException();
        }

        public void Dispose()
        {
            UdpClient?.Close();
            UdpClient?.Dispose();
        }
    }
}
