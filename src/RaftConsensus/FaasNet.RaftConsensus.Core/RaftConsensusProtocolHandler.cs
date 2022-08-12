using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.Messages;
using FaasNet.RaftConsensus.Core.Infos;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
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
            return result;
        }

        private Task<BaseConsensusPackage> Handle(VoteRequest request, CancellationToken cancellationToken)
        {
            bool isGranted = true;
            if (_peerInfo.IsLeaderActive(_raftConsensusPeerOptions.LeaderHeartbeatExpirationDurationMS)) isGranted = false;
            else if (request.Term < _peerState.CurrentTerm) isGranted = false;
            else if (request.LastLogIndex < _peerState.LastApplied) isGranted = false;
            return Task.FromResult(ConsensusPackageResultBuilder.Vote(request.Term, isGranted));
        }

        private async Task<BaseConsensusPackage> Handle(AppendEntriesRequest request, CancellationToken cancellationToken)
        {
            bool success = true;
            LogEntry logEntry;
            _peerInfo.MoveToFollower();
            if(request.Term < _peerState.CurrentTerm) success = false;
            else if ((logEntry = await _logStore.Get(request.PreLogTerm, request.PrevLogIndex, cancellationToken)) == null) success = false;
            else
            {
                await UpdateLogEntries(request, cancellationToken);
                UpdateCommitIndex(request, _peerState);
            }

            return ConsensusPackageResultBuilder.AppendEntries(_peerState.CurrentTerm, _peerState.CommitIndex, success);
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
        }
    }
}
