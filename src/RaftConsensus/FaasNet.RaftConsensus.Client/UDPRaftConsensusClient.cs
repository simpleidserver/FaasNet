using FaasNet.Common.Helpers;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client.Messages;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Client
{
    public class UDPRaftConsensusClient : BasePartitionedPeerClient, IRaftConsensusClient
    {
        public UDPRaftConsensusClient(IPEndPoint target) : base(target)
        {
            UdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        }

        public UDPRaftConsensusClient(string url, int port) : base(new IPEndPoint(DnsHelper.ResolveIPV4(url), port))
        {
            UdpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        }

        public UdpClient UdpClient { get; private set; }

        public async Task<IEnumerable<VoteResult>> Vote(string candidateId, long currentTerm, long commitIndex, long lastApplied, CancellationToken cancellationToken)
        {
            var request = SerializeRequest(ConsensusPackageRequestBuilder.Vote(candidateId, currentTerm, commitIndex, lastApplied));
            await UdpClient.SendAsync(request, Target, cancellationToken);
            var receivedResult = await UdpClient.ReceiveAsync(cancellationToken);
            return DeserializeResult<BaseConsensusPackage, VoteResult>(receivedResult.Buffer);
        }

        public async Task<IEnumerable<AppendEntriesResult>> Heartbeat(long currentTerm, string candidateId, long commitIndex, CancellationToken cancellationToken)
        {
            var request = SerializeRequest(ConsensusPackageRequestBuilder.Heartbeat(currentTerm, candidateId, commitIndex));
            await UdpClient.SendAsync(request, Target, cancellationToken);
            var receivedResult = await UdpClient.ReceiveAsync(cancellationToken);
            return DeserializeResult<BaseConsensusPackage, AppendEntriesResult>(receivedResult.Buffer);
        }

        public async Task<IEnumerable<AppendEntriesResult>> AppendEntries(long term, string leaderId, long prevLogIndex, long prevLogTerm, IEnumerable<LogEntry> entries, long leaderCommit, CancellationToken cancellationToken)
        {
            var request = SerializeRequest(ConsensusPackageRequestBuilder.AppendEntries(term, leaderId, prevLogIndex, prevLogTerm, entries, leaderCommit));
            await UdpClient.SendAsync(request, Target, cancellationToken);
            var receivedResult = await UdpClient.ReceiveAsync(cancellationToken);
            return DeserializeResult<BaseConsensusPackage, AppendEntriesResult>(receivedResult.Buffer);
        }

        public async Task<IEnumerable<AppendEntryResult>> AppendEntry(byte[] payload, CancellationToken cancellationToken)
        {
            var request = SerializeRequest(ConsensusPackageRequestBuilder.AppendEntry(payload));
            await UdpClient.SendAsync(request, Target, cancellationToken);
            var receivedResult = await UdpClient.ReceiveAsync(cancellationToken);
            return DeserializeResult<BaseConsensusPackage, AppendEntryResult>(receivedResult.Buffer);
        }

        public async Task<IEnumerable<GetPeerStateResult>> GetPeerState(CancellationToken cancellationToken)
        {
            var request = SerializeRequest(ConsensusPackageRequestBuilder.GetPeerState());
            await UdpClient.SendAsync(request, Target, cancellationToken);
            var receivedResult = await UdpClient.ReceiveAsync(cancellationToken);
            return DeserializeResult<BaseConsensusPackage, GetPeerStateResult>(receivedResult.Buffer);
        }

        public async Task<IEnumerable<GetLogsResult>> GetLogs(int index, CancellationToken cancellationToken)
        {
            var request = SerializeRequest(ConsensusPackageRequestBuilder.GetLogs(index));
            await UdpClient.SendAsync(request, Target, cancellationToken);
            var receivedResult = await UdpClient.ReceiveAsync(cancellationToken);
            return DeserializeResult<BaseConsensusPackage, GetLogsResult>(receivedResult.Buffer);
        }

        public override void Dispose()
        {
            UdpClient?.Dispose();
        }
    }
}
