﻿using FaasNet.RaftConsensus.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Client.Messages.Gossip;
using System;
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

        public Task Heartbeat(string url, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            var package = GossipPackageRequestBuilder.Heartbeat(url, port);
            return Send(package, cancellationToken);
        }

        public async Task Send(GossipPackage gossipPackage, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            gossipPackage.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await UdpClient.SendAsync(payload, payload.Count(), _target).WithCancellation(cancellationToken);
        }

        public Task JoinNode(string url, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            var package = GossipPackageRequestBuilder.AddNode(url, port);
            return Send(package, cancellationToken);
        }

        public void Dispose()
        {
            UdpClient?.Dispose();
        }
    }
}
