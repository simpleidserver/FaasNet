using FaasNet.Common.Extensions;
using FaasNet.Common.Helpers;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client.Messages;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Client
{
    public class UDPRaftConsensusClient: IDisposable
    {
        private readonly IPEndPoint _target;

        public UDPRaftConsensusClient(IPEndPoint target)
        {
            _target = target;
            UdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        }

        public UDPRaftConsensusClient(string url, int port) : this(new IPEndPoint(DnsHelper.ResolveIPV4(url), port)) { }

        public UdpClient UdpClient { get; private set; }

        public async Task LeaderHeartbeat(string termId, long termIndex, string url, int port, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = ConsensusPackageRequestBuilder.LeaderHeartbeat(url, port, termId, termIndex);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await UdpClient.SendAsync(payload, payload.Count(), _target).WithCancellation(cancellationToken);
        }

        public async Task AppendEntry(string termId, string value, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = ConsensusPackageRequestBuilder.AppendEntry(termId, 0, value, false);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await UdpClient.SendAsync(payload, payload.Count(), _target).WithCancellation(cancellationToken);
            await UdpClient.ReceiveAsync().WithCancellation(cancellationToken);
        }

        public async Task<GetEntryResult> GetEntry(string termId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var writeCtx = new WriteBufferContext();
            var package = ConsensusPackageRequestBuilder.GetEntry(termId);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await UdpClient.SendAsync(payload, payload.Count(), _target).WithCancellation(cancellationToken);
            var resultPayload = await UdpClient.ReceiveAsync().WithCancellation(cancellationToken);
            var readCtx = new ReadBufferContext(resultPayload.Buffer);
            var result = BaseConsensusPackage.Deserialize(readCtx, false);
            return result as GetEntryResult;
        }

        public async Task Vote(string url, int port, string termId, long termIndex, CancellationToken cancellationToken)
        {
            var writeCtx = new WriteBufferContext();
            var package = ConsensusPackageRequestBuilder.Vote(url, port, termId, termIndex);
            package.SerializeEnvelope(writeCtx);
            var payload = writeCtx.Buffer.ToArray();
            await UdpClient.SendAsync(payload, payload.Count(), _target).WithCancellation(cancellationToken);
        }

        public void Dispose()
        {
            UdpClient?.Dispose();
        }
    }
}
