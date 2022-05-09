using FaasNet.RaftConsensus.Client.Extensions;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Client.Messages.Consensus;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Client
{
    public class ConsensusClient: IDisposable
    {
        private readonly IPEndPoint _target;

        public ConsensusClient(IPEndPoint target)
        {
            _target = target;
            UdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        }

        public ConsensusClient(string url, int port) : this(new IPEndPoint(IPAddressHelper.ResolveIPAddress(url), port)) { }

        public UdpClient UdpClient { get; private set; }

        public async Task LeaderHeartbeat(string termId, long termIndex, string url, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = ConsensusPackageRequestBuilder.LeaderHeartbeat(url, port, termId, termIndex);
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await UdpClient.SendAsync(payload, payload.Count(), _target).WithCancellation(cancellationToken);
        }

        public async Task AppendEntry(string termId, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = ConsensusPackageRequestBuilder.AppendEntry(termId, 0, value);
            package.Serialize(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await UdpClient.SendAsync(payload, payload.Count(), _target).WithCancellation(cancellationToken);
        }

        public void Dispose()
        {
            UdpClient?.Dispose();
        }
    }
}
