using FaasNet.Common;
using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Infos;
using FaasNet.RaftConsensus.Core.StateMachines;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IPeerClientFactory _peerClientFactory;
        private readonly ICommitHelper _commitHelper;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISnapshotHelper _snapshotHelper;
        private readonly RaftConsensusPeerOptions _raftConsensusPeerOptions;
        private readonly ILogger<RaftConsensusProtocolHandler> _logger;
        private readonly TaskPool _taskPool;
        private SemaphoreSlim _appendEntrySem = new SemaphoreSlim(1);

        public RaftConsensusProtocolHandler(ILogger<RaftConsensusProtocolHandler>  logger, IPeerInfoStore peerInfoStore, ILogStore logStore, IPeerClientFactory peerClientFactory, ICommitHelper commitHelper, IServiceProvider serviceProvider, ISnapshotHelper snapshotHelper, IOptions<RaftConsensusPeerOptions> raftConsensusPeerOptions)
        {
            _logger = logger;
            _peerInfo = peerInfoStore.Get();
            _logStore = logStore;
            _raftConsensusPeerOptions = raftConsensusPeerOptions.Value;
            _peerClientFactory = peerClientFactory;
            _commitHelper = commitHelper;
            _serviceProvider = serviceProvider;
            _snapshotHelper = snapshotHelper;
            _peerState = PeerState.New(raftConsensusPeerOptions.Value.ConfigurationDirectoryPath, raftConsensusPeerOptions.Value.IsConfigurationStoredInMemory);
            _taskPool = new TaskPool(_raftConsensusPeerOptions.MaxNbThreads);
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
            if (consensusPackage.Command == ConsensusCommands.INSTALL_SNAPSHOT_REQUEST) result = await Handle(consensusPackage as InstallSnapshotRequest, cancellationToken);
            if (consensusPackage.Command == ConsensusCommands.QUERY_REQUEST) result = await Handle(consensusPackage as QueryRequest, cancellationToken);
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
                await UpdateCommitIndex(request, _peerState);
                UpdateTerm(request, _peerState);
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

            async Task UpdateCommitIndex(AppendEntriesRequest request, PeerState peerState)
            {
                if (request.LeaderCommit > peerState.CommitIndex && request.LeaderCommit == peerState.LastApplied)
                {
                    Debug.WriteLine($"{request.LeaderCommit} > {peerState.CommitIndex} {request.PrevLogIndex}");
                    await _commitHelper.Commit(request.LeaderCommit, cancellationToken);
                }
            }

            void UpdateTerm(AppendEntriesRequest request, PeerState peerState)
            {
                if (request.Term > peerState.CurrentTerm)
                {
                    peerState.CurrentTerm = request.Term;
                    peerState.VotedFor = request.LeaderId;
                }
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
                if (!_peerInfo.IsLeaderActive(_raftConsensusPeerOptions.LeaderHeartbeatExpirationDurationMS) || string.IsNullOrWhiteSpace(_peerState.VotedFor)) return ConsensusPackageResultBuilder.AppendEntry(0, 0, 0, false);
                var leaderPeerId = PeerId.Deserialize(_peerState.VotedFor);
                using (var consensusClient = _peerClientFactory.Build<RaftConsensusClient>(leaderPeerId.IpEdp))
                    return (await consensusClient.AppendEntry(request.Payload, _raftConsensusPeerOptions.RequestExpirationTimeMS, cancellationToken)).First().Item1;
            }

            async Task<BaseConsensusPackage> Append(AppendEntryRequest request, CancellationToken cancellationToken)
            {
                _appendEntrySem.Wait();
                var lastIndex = _peerState.LastApplied;
                var logEntry = new LogEntry
                {
                    Term = _peerState.CurrentTerm,
                    Command = request.Payload,
                    Index = lastIndex + 1
                };
                await _logStore.Append(logEntry, cancellationToken);
                _peerState.IncreaseLastIndex();
                _appendEntrySem.Release();
                return ConsensusPackageResultBuilder.AppendEntry(_peerState.CurrentTerm, _peerState.CommitIndex, _peerState.LastApplied, true);
            }
        }

        private Task<BaseConsensusPackage> Handle(GetPeerStateRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(ConsensusPackageResultBuilder.GetPeerState(_peerState.CurrentTerm, _peerState.VotedFor, _peerState.CommitIndex, _peerState.LastApplied, _peerInfo.Status, _peerState.SnapshotLastApplied, _peerState.SnapshotCommitIndex));
        }

        private async Task<BaseConsensusPackage> Handle(GetLogsRequest request, CancellationToken cancellationToken)
        {
            var result = await _logStore.GetFrom(request.StartIndex, cancellationToken: cancellationToken);
            return ConsensusPackageResultBuilder.GetLogs(result);
        }

        private Task<BaseConsensusPackage> Handle(InstallSnapshotRequest request, CancellationToken cancellationToken)
        {
            bool success = false;
            if (request.SnapshotTerm < _peerState.CurrentTerm || request.SnapshotIndex < _peerState.CommitIndex) success = false;
            else
            {
                success = true;
                if(request.IsFinished)
                {
#pragma warning disable 4014
                    _taskPool.Enqueue(() => RestoreStateMachine(_peerState));
#pragma warning restore 4014
                }
                else UpdateSnapshot(request);
            }

            return Task.FromResult(ConsensusPackageResultBuilder.InstallSnapshot(success));

            void UpdateSnapshot(InstallSnapshotRequest request)
            {
                var newIndex = _peerState.SnapshotCommitIndex + 1;
                if (request.IsInit) _snapshotHelper.EraseSnapshot(newIndex);
                _snapshotHelper.WriteSnapshot(newIndex, request.RecordIndex, request.Data);
            }

            async Task RestoreStateMachine(PeerState peerState)
            {
                var newIndex = _peerState.SnapshotCommitIndex + 1;
                var stateMachine = (IStateMachine)ActivatorUtilities.CreateInstance(_serviceProvider, _raftConsensusPeerOptions.StateMachineType);
                await stateMachine.Truncate(cancellationToken);
                var allRecords = new List<IRecord>();
                foreach (var snapshotRecord in _snapshotHelper.ReadSnapshot(newIndex))
                {
                    var readBufferContext = new ReadBufferContext(snapshotRecord.Content.ToArray());
                    var record = (IRecord)Activator.CreateInstance(Type.GetType(readBufferContext.NextString()));
                    record.Deserialize(readBufferContext);
                    allRecords.Add(record);
                }

                await stateMachine.BulkUpload(allRecords, cancellationToken);
                await CommitSnapshot(peerState, stateMachine);
            }

            async Task CommitSnapshot(PeerState peerState, IStateMachine stateMachine)
            {
                peerState.SnapshotCommitIndex = peerState.SnapshotCommitIndex + 1;
                peerState.SnapshotLastApplied = request.SnapshotIndex;
                peerState.SnapshotTerm = request.SnapshotTerm;
                peerState.CommitIndex = peerState.SnapshotLastApplied;
                peerState.LastApplied = peerState.SnapshotLastApplied;
                _logger.LogInformation("Snapshot {stateMachine} is committed, last log index is {snapshotLastApplied}", stateMachine.Name, peerState.SnapshotLastApplied);
                await _logStore.Truncate(CancellationToken.None);
            }
        }

        private async Task<BaseConsensusPackage> Handle(QueryRequest request, CancellationToken cancellationToken)
        {
            var stateMachine = (IStateMachine)ActivatorUtilities.CreateInstance(_serviceProvider, _raftConsensusPeerOptions.StateMachineType);
            var result = await stateMachine.Query(request.Query, cancellationToken);
            return ConsensusPackageResultBuilder.Query(result);
        }
    }
}
