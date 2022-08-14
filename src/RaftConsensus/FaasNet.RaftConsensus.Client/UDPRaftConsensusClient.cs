using FaasNet.Common.Helpers;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client.Messages;
using System;
using System.Collections.Generic;
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

        public async Task<VoteResult> Vote(string candidateId, long currentTerm, long commitIndex, long lastApplied, CancellationToken cancellationToken)
        {
            var writeBufferCtx = new WriteBufferContext();
            var pkg = ConsensusPackageRequestBuilder.Vote(candidateId, currentTerm, commitIndex, lastApplied);
            pkg.SerializeEnvelope(writeBufferCtx);
            await UdpClient.SendAsync(writeBufferCtx.Buffer.ToArray(), _target, cancellationToken);
            var receivedResult = await UdpClient.ReceiveAsync(cancellationToken);
            var readBufferCtx = new ReadBufferContext(receivedResult.Buffer);
            return BaseConsensusPackage.Deserialize(readBufferCtx) as VoteResult;
        }

        public async Task<AppendEntriesResult> Heartbeat(long currentTerm, string candidateId, long commitIndex, CancellationToken cancellationToken)
        {
            var writeBufferCtx = new WriteBufferContext();
            var pkg = ConsensusPackageRequestBuilder.Heartbeat(currentTerm, candidateId, commitIndex);
            pkg.SerializeEnvelope(writeBufferCtx);
            await UdpClient.SendAsync(writeBufferCtx.Buffer.ToArray(), _target, cancellationToken);
            var receivedResult = await UdpClient.ReceiveAsync(cancellationToken);
            var readBufferCtx = new ReadBufferContext(receivedResult.Buffer);
            return BaseConsensusPackage.Deserialize(readBufferCtx) as AppendEntriesResult;
        }

        public async Task<AppendEntriesResult> AppendEntries(long term, string leaderId, long prevLogIndex, long prevLogTerm, IEnumerable<LogEntry> entries, long leaderCommit, CancellationToken cancellationToken)
        {
            var writeBufferCtx = new WriteBufferContext();
            var pkg = ConsensusPackageRequestBuilder.AppendEntries(term, leaderId, prevLogIndex, prevLogTerm, entries, leaderCommit);
            pkg.SerializeEnvelope(writeBufferCtx);
            await UdpClient.SendAsync(writeBufferCtx.Buffer.ToArray(), _target, cancellationToken);
            var receivedResult = await UdpClient.ReceiveAsync(cancellationToken);
            var readBufferCtx = new ReadBufferContext(receivedResult.Buffer);
            return BaseConsensusPackage.Deserialize(readBufferCtx) as AppendEntriesResult;
        }

        public async Task<AppendEntryResult> AppendEntry(byte[] payload, CancellationToken cancellationToken)
        {
            var writeBufferCtx = new WriteBufferContext();
            var pkg = ConsensusPackageRequestBuilder.AppendEntry(payload);
            pkg.SerializeEnvelope(writeBufferCtx);
            await UdpClient.SendAsync(writeBufferCtx.Buffer.ToArray(), _target, cancellationToken);
            var receivedResult = await UdpClient.ReceiveAsync(cancellationToken);
            var readBufferCtx = new ReadBufferContext(receivedResult.Buffer);
            return BaseConsensusPackage.Deserialize(readBufferCtx) as AppendEntryResult;
        }

        public void Dispose()
        {
            UdpClient?.Dispose();
        }
    }
}
