﻿using FaasNet.Peer.Client;
using FaasNet.Peer.Client.Transports;
using FaasNet.RaftConsensus.Client.Messages;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Client
{
    public class RaftConsensusClient : BasePartitionedPeerClient
    {
        public RaftConsensusClient(IClientTransport clientTransport) : base(clientTransport) { }

        public UdpClient UdpClient { get; private set; }

        public async Task<IEnumerable<VoteResult>> Vote(string candidateId, long currentTerm, long commitIndex, long lastApplied, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = SerializeRequest(ConsensusPackageRequestBuilder.Vote(candidateId, currentTerm, commitIndex, lastApplied));
            await Send(request, timeoutMS, cancellationToken);
            var receivedResult = await Receive(timeoutMS, cancellationToken);
            return DeserializeResult<BaseConsensusPackage, VoteResult>(receivedResult);
        }

        public async Task<IEnumerable<AppendEntriesResult>> Heartbeat(long currentTerm, string candidateId, long commitIndex, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = SerializeRequest(ConsensusPackageRequestBuilder.Heartbeat(currentTerm, candidateId, commitIndex));
            await Send(request, timeoutMS, cancellationToken);
            var receivedResult = await Receive(timeoutMS, cancellationToken);
            return DeserializeResult<BaseConsensusPackage, AppendEntriesResult>(receivedResult);
        }

        public async Task<IEnumerable<AppendEntriesResult>> AppendEntries(long term, string leaderId, long prevLogIndex, long prevLogTerm, IEnumerable<LogEntry> entries, long leaderCommit, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = SerializeRequest(ConsensusPackageRequestBuilder.AppendEntries(term, leaderId, prevLogIndex, prevLogTerm, entries, leaderCommit));
            await Send(request, timeoutMS, cancellationToken);
            var receivedResult = await Receive(timeoutMS, cancellationToken);
            return DeserializeResult<BaseConsensusPackage, AppendEntriesResult>(receivedResult);
        }

        public async Task<IEnumerable<AppendEntryResult>> AppendEntry(byte[] payload, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = SerializeRequest(ConsensusPackageRequestBuilder.AppendEntry(payload));
            await Send(request, timeoutMS, cancellationToken);
            var receivedResult = await Receive(timeoutMS, cancellationToken);
            return DeserializeResult<BaseConsensusPackage, AppendEntryResult>(receivedResult);
        }

        public async Task<IEnumerable<GetPeerStateResult>> GetPeerState(int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = SerializeRequest(ConsensusPackageRequestBuilder.GetPeerState());
            await Send(request, timeoutMS, cancellationToken);
            var receivedResult = await Receive(timeoutMS, cancellationToken);
            return DeserializeResult<BaseConsensusPackage, GetPeerStateResult>(receivedResult);
        }

        public async Task<IEnumerable<GetLogsResult>> GetLogs(int index, int timeoutMS = 500, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = SerializeRequest(ConsensusPackageRequestBuilder.GetLogs(index));
            await Send(request, timeoutMS, cancellationToken);
            var receivedResult = await Receive(timeoutMS, cancellationToken);
            return DeserializeResult<BaseConsensusPackage, GetLogsResult>(receivedResult);
        }
    }
}
