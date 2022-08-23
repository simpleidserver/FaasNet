using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Infos;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
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
        private readonly RaftConsensusPeerOptions _raftConsensusPeerOptions;

        public RaftConsensusProtocolHandler(IPeerInfoStore peerInfoStore, ILogStore logStore, IOptions<RaftConsensusPeerOptions> raftConsensusPeerOptions)
        {
            _peerInfo = peerInfoStore.Get();
            _logStore = logStore;
            _raftConsensusPeerOptions = raftConsensusPeerOptions.Value;
            _peerState = PeerState.New(raftConsensusPeerOptions.Value.ConfigurationDirectoryPath);
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
            if (consensusPackage.Command == ConsensusCommands.GET_PEER_STATE_REQUEST) result = await Handle(consensusPackage as GetPeerStateRequest, cancellationToken);
            if (consensusPackage.Command == ConsensusCommands.GET_LOGS_REQUEST) result = await Handle(consensusPackage as GetLogsRequest, cancellationToken);
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
            return ConsensusPackageResultBuilder.AppendEntries(_peerState.CurrentTerm, _peerState.LastApplied, success);
            async Task UpdateLogEntries(AppendEntriesRequest request, CancellationToken cancellationToken)
            {
                if (!request.Entries.Any()) return;
                var conflictedLog = await GetFirstConflictedLog(request, cancellationToken);
                if (conflictedLog.Item1 != null)
                    await _logStore.RemoveFrom(conflictedLog.Item1.Value, cancellationToken);
                await _logStore.UpdateRange(request.Entries, cancellationToken);
                _peerState.LastApplied = conflictedLog.Item2.Value;
            }

            async Task<(long?, long?)> GetFirstConflictedLog(AppendEntriesRequest request, CancellationToken cancellationToken)
            {
                var dic = request.Entries.Select(x => (x, x.Index)).OrderBy(i => i.Index);
                long? startFromIndex = null;
                foreach (var kvp in dic)
                {
                    var logEntry = await _logStore.Get(kvp.Index, cancellationToken);
                    if (logEntry != null && logEntry.Term != kvp.x.Term)
                    {
                        startFromIndex = kvp.Index;
                        break;
                    }
                }

                return (startFromIndex, dic.Last().Index);
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
                    return (await consensusClient.AppendEntry(request.Payload, cancellationToken)).First();
            }

            async Task<BaseConsensusPackage> Append(AppendEntryRequest request, CancellationToken cancellationToken)
            {
                var lastIndex = _peerState.LastApplied;
                var logEntry = new LogEntry
                {
                    Term = _peerState.CurrentTerm,
                    Command = request.Payload,
                    Index = lastIndex + 1
                };
                _peerState.IncreaseLastIndex();
                await _logStore.Append(logEntry, cancellationToken);
                return ConsensusPackageResultBuilder.AppendEntry(_peerState.CurrentTerm, _peerState.CommitIndex, true);
            }
        }

        private Task<BaseConsensusPackage> Handle(GetPeerStateRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(ConsensusPackageResultBuilder.GetPeerState(_peerState.CurrentTerm, _peerState.VotedFor, _peerState.CommitIndex, _peerState.LastApplied, _peerInfo.Status));
        }

        private async Task<BaseConsensusPackage> Handle(GetLogsRequest request, CancellationToken cancellationToken)
        {
            var result = await _logStore.GetFrom(request.StartIndex, cancellationToken);
            return ConsensusPackageResultBuilder.GetLogs(result);
        }
    }
}
