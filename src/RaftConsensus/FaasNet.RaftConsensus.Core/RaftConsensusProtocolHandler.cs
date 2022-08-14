using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Infos;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    /// <summary>
    /// Documentation : https://raft.github.io/raft.pdf
    /// </summary>
    public class RaftConsensusProtocolHandler : IProtocolHandler
    {
        private readonly PeerInfo _peerInfo;
        private readonly PeerState _peerState;
        private readonly ILogStore _logStore;
        private readonly PeerOptions _peerOptions;
        private readonly RaftConsensusPeerOptions _raftConsensusPeerOptions;
        private readonly ILogger<RaftConsensusProtocolHandler> _logger;

        public RaftConsensusProtocolHandler(IPeerInfoStore peerInfoStore, ILogStore logStore, IOptions<PeerOptions> peerOptions, IOptions<RaftConsensusPeerOptions> raftConsensusPeerOptions, ILogger<RaftConsensusProtocolHandler> logger)
        {
            _peerInfo = peerInfoStore.Get();
            _logStore = logStore;
            _peerOptions = peerOptions.Value;
            _raftConsensusPeerOptions = raftConsensusPeerOptions.Value;
            _peerState = PeerState.New(raftConsensusPeerOptions.Value.ConfigurationDirectoryPath);
            _logger = logger;
        }

        public string MagicCode => BaseConsensusPackage.MAGIC_CODE;

        public async Task<BasePeerPackage> Handle(byte[] payload, CancellationToken cancellationToken)
        {
            var bufferContext = new ReadBufferContext(payload);
            var consensusPackage = BaseConsensusPackage.Deserialize(bufferContext, true);
            BaseConsensusPackage result = null;
            if (consensusPackage.Command == ConsensusCommands.VOTE_REQUEST) result = await Handle(consensusPackage as VoteRequest, cancellationToken);
            if (consensusPackage.Command == ConsensusCommands.APPEND_ENTRIES_REQUEST) result = await Handle(consensusPackage as AppendEntriesRequest, cancellationToken);
            if (consensusPackage.Command == ConsensusCommands.APPEND_ENTRY_REQUEST) result = await Handle(consensusPackage as AppendEntryRequest, cancellationToken);
            return result;
        }

        private Task<BaseConsensusPackage> Handle(VoteRequest request, CancellationToken cancellationToken)
        {
            bool isGranted = true;
            if (request.Term < _peerState.CurrentTerm ||
                request.LastLogIndex < _peerState.LastApplied ||
                (!string.IsNullOrWhiteSpace(_peerState.VotedFor) && request.Term == _peerState.CurrentTerm)) isGranted = false;
            if (request.Term > _peerState.CurrentTerm)
            {
                _peerInfo.MoveToFollower();
                _peerState.CurrentTerm = request.Term;
            }

            if (isGranted) _peerState.VotedFor = request.CandidateId;
            return Task.FromResult(ConsensusPackageResultBuilder.Vote(request.Term, isGranted));
        }

        private async Task<BaseConsensusPackage> Handle(AppendEntriesRequest request, CancellationToken cancellationToken)
        {
            bool success = true;
            LogEntry logEntry;
            _peerInfo.MoveToFollower();
            if(request.Term < _peerState.CurrentTerm) success = false;
            else if (request.PrevLogIndex != default(long) && (logEntry = await _logStore.Get(request.PreLogTerm, request.PrevLogIndex, cancellationToken)) == null) success = false;
            else
            {
                await UpdateLogEntries(request, cancellationToken);
                UpdateCommitIndex(request, _peerState);
            }

            UpdateLeader(_peerInfo);
            var matchIndex = await _logStore.GetLastIndex(cancellationToken);
            return ConsensusPackageResultBuilder.AppendEntries(_peerState.CurrentTerm, matchIndex, success);
            async Task UpdateLogEntries(AppendEntriesRequest request, CancellationToken cancellationToken)
            {
                logEntry = await _logStore.Get(request.LeaderCommit, cancellationToken);
                if (logEntry != null && (logEntry.Index == request.LeaderCommit && logEntry.Term != request.Term)) await _logStore.RemoveFrom(request.LeaderCommit, cancellationToken);
                await _logStore.UpdateRange(request.Entries, cancellationToken);           
            }

            void UpdateCommitIndex(AppendEntriesRequest request, PeerState peerState)
            {
                if (request.LeaderCommit > peerState.CommitIndex) peerState.CommitIndex = request.LeaderCommit;
            }

            void UpdateLeader(PeerInfo peerInfo)
            {
                peerInfo.LeaderHeartbeatReceptionDateTime = DateTime.UtcNow;
            }
        }

        private async Task<BaseConsensusPackage> Handle(AppendEntryRequest request, CancellationToken cancellationToken)
        {
            if (_peerInfo.Status != PeerStatus.LEADER) return await Transfer(request, cancellationToken);
            return await Append(request, cancellationToken);

            async Task<BaseConsensusPackage> Transfer(AppendEntryRequest request, CancellationToken cancellationToken)
            {
                if (!_peerInfo.IsLeaderActive(_raftConsensusPeerOptions.LeaderHeartbeatExpirationDurationMS)) return ConsensusPackageResultBuilder.AppendEntry(0, 0, false);
                var leaderPeerId = PeerId.Deserialize(_peerState.VotedFor);
                using (var consensusClient = new UDPRaftConsensusClient(leaderPeerId.IpEdp))
                    return await consensusClient.AppendEntry(request.Payload, cancellationToken);
            }

            async Task<BaseConsensusPackage> Append(AppendEntryRequest request, CancellationToken cancellationToken)
            {
                var lastIndex = await _logStore.GetLastIndex(cancellationToken);
                var logEntry = new LogEntry
                {
                    Term = _peerState.CurrentTerm,
                    Command = request.Payload,
                    Index = lastIndex + 1
                };
                await _logStore.Append(logEntry, cancellationToken);
                return ConsensusPackageResultBuilder.AppendEntry(_peerState.CurrentTerm, _peerState.CommitIndex, true);
            }
        }
    }
}
