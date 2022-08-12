namespace FaasNet.RaftConsensus.Core
{
    public partial class RaftConsensusTimer
    {
        private System.Timers.Timer _followerTimer;

        private void CreateFollowerTimer()
        {
            _followerTimer = new System.Timers.Timer();
            _followerTimer.Elapsed += (o, e) => CheckHeartbeat();
            _followerTimer.AutoReset = false;
        }

        private void StartFollower()
        {
            _followerTimer.Interval = _raftOptions.ElectionTimer.GetValue();
            _followerTimer?.Start();
        }

        private void StopFollowerTimer()
        {
            _followerTimer.Stop();
        }

        private void CheckHeartbeat()
        {
            if (_peerInfo.IsLeaderActive(_raftOptions.LeaderHeartbeatExpirationDurationMS))
            {
                _peerInfo.MoveToFollower();
                return;
            }

            _peerInfo.MoveToCandidate();
        }
    }
}
