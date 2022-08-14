using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using FaasNet.RaftConsensus.Core.Infos;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly PeerOptions _peerOptions;
        private readonly RaftConsensusPeerOptions _raftOptions;
        private PeerInfo _peerInfo;
        private PeerState _peerState;
        private CancellationTokenSource _cancellationTokenSource;

        public RaftConsensusTimer(IPeerInfoStore peerInfoStore, ILogger<RaftConsensusTimer> logger, IClusterStore clusterStore, ILogStore logStore, IOptions<PeerOptions> peerOptions, IOptions<RaftConsensusPeerOptions> raftOptions)
        {
            _peerInfoStore = peerInfoStore;
            _logger = logger;
            _clusterStore = clusterStore;
            _logStore = logStore;
            _peerOptions = peerOptions.Value;
            _raftOptions = raftOptions.Value;
            CreateFollowerTimer();
            CreateLeaderTimer();
        }

        public Task Start(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _peerInfo = _peerInfoStore.Get();
            _peerState = PeerState.New(_raftOptions.ConfigurationDirectoryPath);
            _peerInfo.FollowerStateStarted += (sender, args) =>
            {
                _logger.LogInformation($"Peer {_peerOptions.Id} is a follower");
                StartFollower();
            };
            _peerInfo.CandidateStateStarted += async (sender, args) =>
            {
                _logger.LogInformation($"Peer {_peerOptions.Id} is a candidate");
                await StartCandidate();
            };
            _peerInfo.LeaderStateStarted += (sender, args) =>
            {
                _logger.LogInformation($"Peer {_peerOptions.Id} is a leader");
                StartLeader();
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
