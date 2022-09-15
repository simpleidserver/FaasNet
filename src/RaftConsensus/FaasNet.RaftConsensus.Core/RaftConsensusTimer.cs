using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Core.Infos;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public partial class RaftConsensusTimer : ITimer
    {
        private readonly IPeerInfoStore _peerInfoStore;
        private readonly ILogger<RaftConsensusTimer> _logger;
        private readonly IClusterStore _clusterStore;
        private readonly ILogStore _logStore;
        private readonly IPeerClientFactory _peerClientFactory;
        private readonly ICommitHelper _commitHelper;
        private readonly ISnapshotHelper _snapshotHelper;
        private readonly PeerOptions _peerOptions;
        private readonly RaftConsensusPeerOptions _raftOptions;
        private PeerInfo _peerInfo;
        private PeerState _peerState;
        private CancellationTokenSource _cancellationTokenSource;

        public RaftConsensusTimer(IPeerInfoStore peerInfoStore, ILogger<RaftConsensusTimer> logger, IClusterStore clusterStore, ILogStore logStore, IPeerClientFactory peerClientFactory, ICommitHelper commitHelper, ISnapshotHelper snapshotHelper, IOptions<PeerOptions> peerOptions, IOptions<RaftConsensusPeerOptions> raftOptions)
        {
            _peerInfoStore = peerInfoStore;
            _logger = logger;
            _clusterStore = clusterStore;
            _logStore = logStore;
            _peerClientFactory = peerClientFactory;
            _commitHelper = commitHelper;
            _snapshotHelper = snapshotHelper;
            _peerOptions = peerOptions.Value;
            _raftOptions = raftOptions.Value;
            CreateFollowerTimer();
            CreateLeaderTimer();
        }

        public Task Start(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _peerInfo = _peerInfoStore.Get();
            _peerState = PeerState.New(_raftOptions.ConfigurationDirectoryPath, _raftOptions.IsConfigurationStoredInMemory);
            _peerInfo.FollowerStateStarted += (sender, args) =>
            {
                var stateChanged = args as PeerInfoStateChanged;
                if(stateChanged.IsDifferent) _logger.LogInformation($"Peer {_peerOptions.Id} is a follower");
                StartFollower();
            };
            _peerInfo.CandidateStateStarted += async (sender, args) =>
            {
                _logger.LogInformation($"Peer {_peerOptions.Id} is a candidate");
                await StartCandidate();
            };
            _peerInfo.LeaderStateStarted += async (sender, args) =>
            {
                _logger.LogInformation($"Peer {_peerOptions.Id} is a leader");
                var allPeers = (await _clusterStore.GetAllNodes(_peerOptions.PartitionKey, _cancellationTokenSource.Token)).Where(p => p.Id != _peerOptions.Id);
                await AppendEntries(allPeers);
                StartLeader();
                if (_raftOptions.LeaderCallback != null) _raftOptions.LeaderCallback();
            };
            _peerInfo.MoveToFollower();
            return Task.CompletedTask;
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            StopFollowerTimer();
            StopLeaderTimer();
        }
    }
}
