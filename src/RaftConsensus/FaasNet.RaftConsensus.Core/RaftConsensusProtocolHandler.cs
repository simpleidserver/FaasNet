using FaasNet.Common.Extensions;
using FaasNet.Common.Helpers;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Transports;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public class RaftConsensusProtocolHandler : IProtocolHandler
    {
        private readonly ILogger<RaftConsensusProtocolHandler> _logger;
        private readonly IPartitionElectionStore _partitionElectionStore;
        private readonly ILeaderPeerInfoStore _leaderPeerInfoStore;
        private readonly ILogStore _logStore;
        private readonly ITransport _transport;
        private readonly PeerOptions _peerOptions;
        private readonly RaftConsensusPeerOptions _raftConsensusPeerOptions;
        private ConcurrentBag<AppendEntryRequest> _appendEntryRequestLst;

        public RaftConsensusProtocolHandler(ILogger<RaftConsensusProtocolHandler> logger, IPartitionElectionStore partitionElectionStore, ILeaderPeerInfoStore leaderPeerInfoStore, ILogStore logStore, ITransport transport, IOptions<PeerOptions> peerOptions, IOptions<RaftConsensusPeerOptions> raftConsensusPeerOptions)
        {
            _logger = logger;
            _partitionElectionStore = partitionElectionStore;
            _leaderPeerInfoStore = leaderPeerInfoStore;
            _logStore = logStore;
            _transport = transport;
            _peerOptions = peerOptions.Value;
            _raftConsensusPeerOptions = raftConsensusPeerOptions.Value;
            _appendEntryRequestLst = new ConcurrentBag<AppendEntryRequest>();
        }

        public string MagicCode => BaseConsensusPackage.MAGIC_CODE;

        public async Task<BasePeerPackage> Handle(byte[] payload, CancellationToken cancellationToken)
        {
            var bufferContext = new ReadBufferContext(payload);
            var consensusPackage = BaseConsensusPackage.Deserialize(bufferContext, true);
            BaseConsensusPackage result = null;
            if (consensusPackage.Header.Command == ConsensusCommands.LEADER_HEARTBEAT_REQUEST) result = await Handle(consensusPackage as LeaderHeartbeatRequest, cancellationToken);
            if (consensusPackage.Header.Command == ConsensusCommands.VOTE_REQUEST) result = await Handle(consensusPackage as VoteRequest, cancellationToken);
            if (consensusPackage.Header.Command == ConsensusCommands.VOTE_RESULT) result = await Handle(consensusPackage as VoteResult, cancellationToken);
            if (consensusPackage.Header.Command == ConsensusCommands.APPEND_ENTRY_REQUEST) result = await Handle(consensusPackage as AppendEntryRequest, cancellationToken);
            if (consensusPackage.Header.Command == ConsensusCommands.LEADER_HEARTBEAT_RESULT) result = await Handle(consensusPackage as LeaderHeartbeatResult, cancellationToken);
            if (consensusPackage.Header.Command == ConsensusCommands.EMPTY_RESULT) result = await Handle(consensusPackage as EmptyConsensusPackage, cancellationToken);
            return result;
        }

        private async Task<BaseConsensusPackage> Handle(LeaderHeartbeatRequest request, CancellationToken cancellationToken)
        {
            var leaderPeers = await _leaderPeerInfoStore.GetAll(cancellationToken);
            var leader = leaderPeers.SingleOrDefault(l => request.Header.TermId == l.PartitionId);
            if(leader == null) leader = new LeaderPeerInfo(request.Header.TermId, request.Header.SourceUrl, request.Header.SourcePort);
            else leader.HeartbeatReceivedDateTime = DateTime.UtcNow;
            await _leaderPeerInfoStore.Update(leader, cancellationToken);
            var tasks = new List<Task>();
            for (var i = 0; i < _appendEntryRequestLst.Count(); i++)
            {
                tasks.Add(new Task(async () =>
                {
                    var appendEntry = _appendEntryRequestLst.ElementAt(i);
                    var edp = new IPEndPoint(DnsHelper.ResolveIPV4(leader.Url), leader.Port);
                    var writeCtx = new WriteBufferContext();
                    appendEntry.SerializeEnvelope(writeCtx);
                    await _transport.Send(writeCtx.Buffer.ToArray(), edp, cancellationToken);
                    _appendEntryRequestLst.Remove(appendEntry);
                }));
            }

            Task.WaitAll(tasks.ToArray());
            var partition = await _partitionElectionStore.Get(request.Header.TermId, cancellationToken);
            if (partition.PeerState == PeerStates.CANDIDATE) SetFollower(partition);
            return ConsensusPackageResultBuilder.LeaderHeartbeat(_peerOptions.Url, _peerOptions.Port, partition.PartitionId, partition.ConfirmedTermIndex);
        }

        private async Task<BaseConsensusPackage> Handle(VoteRequest request, CancellationToken cancellationToken)
        {
            var partition = await _partitionElectionStore.Get(request.Header.TermId, cancellationToken);
            var isGranted = true;
            if (partition.PartitionId != request.Header.TermId) isGranted = false;
            else
            {
                if (request.Header.TermIndex > partition.ConfirmedTermIndex)
                {
                    SetFollower(partition);
                }
                else if (
                    (partition.PeerState == PeerStates.LEADER || partition.PeerState == PeerStates.CANDIDATE) ||
                    (partition.ConfirmedTermIndex >= request.Header.TermIndex)
                )
                {
                    isGranted = false;
                }
            }

            return ConsensusPackageResultBuilder.Vote(_peerOptions.Url, _peerOptions.Port, partition.PartitionId, partition.TermIndex, isGranted);
        }

        private async Task<BaseConsensusPackage> Handle(VoteResult request, CancellationToken cancellationToken)
        {
            var partition = await _partitionElectionStore.Get(request.Header.TermId, cancellationToken);
            if (request.VoteGranted && partition.PeerState == PeerStates.CANDIDATE) partition.NbPositiveVote++;
            return ConsensusPackageResultBuilder.Empty(_peerOptions.Url, _peerOptions.Port, partition.PartitionId, partition.TermIndex);
        }

        private async Task<BaseConsensusPackage> Handle(AppendEntryRequest request, CancellationToken cancellationToken)
        {
            var partition = await _partitionElectionStore.Get(request.Header.TermId, cancellationToken);
            if (partition.PeerState == PeerStates.LEADER || request.IsProxified)
            {
                await AppendEntry(request, partition, cancellationToken);
                return ConsensusPackageResultBuilder.Empty(_peerOptions.Url, _peerOptions.Port, partition.PartitionId, partition.TermIndex);
            }

            var leader = await _leaderPeerInfoStore.Get(request.Header.TermId, cancellationToken);
            // Transfer to leader
            if (leader != null && leader.IsActive(_raftConsensusPeerOptions.LeaderHeartbeatExpirationDurationMS))
            {
                var writeBufferCtx = new WriteBufferContext();
                request.SerializeEnvelope(writeBufferCtx);
                var edp = new IPEndPoint(DnsHelper.ResolveIPV4(leader.Url), leader.Port);
                await _transport.Send(writeBufferCtx.Buffer.ToArray(), edp, cancellationToken);
                return ConsensusPackageResultBuilder.Empty(_peerOptions.Url, _peerOptions.Port, partition.PartitionId, partition.TermIndex);
            }
            else _appendEntryRequestLst.Add(request);
            return ConsensusPackageResultBuilder.Empty(_peerOptions.Url, _peerOptions.Port, partition.PartitionId, partition.TermIndex);
        }

        private async Task<BaseConsensusPackage> Handle(LeaderHeartbeatResult request, CancellationToken cancellationToken)
        {
            var partition = await _partitionElectionStore.Get(request.Header.TermId, cancellationToken);
            if (partition.PeerState != PeerStates.LEADER) return ConsensusPackageResultBuilder.Empty(_peerOptions.Url, _peerOptions.Port, partition.PartitionId, partition.TermIndex);
            if (partition.PartitionId == request.Header.TermId && partition.ConfirmedTermIndex > request.Header.TermIndex)
            {
                var index = request.Header.TermIndex + 1;
                var log = await _logStore.Get(index, cancellationToken);
                if (log == null) return ConsensusPackageResultBuilder.Empty(_peerOptions.Url, _peerOptions.Port, partition.PartitionId, partition.TermIndex);
                var writeBufferCtx = new WriteBufferContext();
                var pkg = ConsensusPackageRequestBuilder.AppendEntry(partition.PartitionId, index, log.Value, true);
                var edp = new IPEndPoint(DnsHelper.ResolveIPV4(request.Header.SourceUrl), request.Header.SourcePort);
                pkg.SerializeEnvelope(writeBufferCtx);
                await _transport.Send(writeBufferCtx.Buffer.ToArray(), edp, cancellationToken);
            }

            return ConsensusPackageResultBuilder.Empty(_peerOptions.Url, _peerOptions.Port, partition.PartitionId, partition.TermIndex);
        }

        private async Task<BaseConsensusPackage> Handle(EmptyConsensusPackage request, CancellationToken cancellationToken)
        {
            var partition = await _partitionElectionStore.Get(request.Header.TermId, cancellationToken);
            return ConsensusPackageResultBuilder.Empty(_peerOptions.Url, _peerOptions.Port, partition.PartitionId, partition.TermIndex);
        }

        private async Task AppendEntry(AppendEntryRequest appendEntry, PartitionElectionRecord partition, CancellationToken cancellationToken)
        {
            if (appendEntry.IsProxified && appendEntry.Header.TermIndex <= partition.ConfirmedTermIndex) return;
            var logRecord = new LogRecord { Value = appendEntry.Value, Index = partition.ConfirmedTermIndex + 1, InsertionDateTime = DateTime.UtcNow };
            _logStore.Add(logRecord);
            partition.Upgrade();
            await _partitionElectionStore.Update(partition, cancellationToken);
        }

        private static void SetFollower(PartitionElectionRecord partition)
        {
            partition.PeerState = PeerStates.FOLLOWER;
            partition.Reset();
            // RESTART TIMER
        }

        private class PartitionInfo
        {
            public string PartitionId { get; set; }
            public PeerStates State { get; set; }
        }
    }
}
