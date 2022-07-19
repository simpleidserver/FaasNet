using FaasNet.Peer;
using FaasNet.RaftConsensus.Core.Stores;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public class RaftConsensusTimer : ITimer
    {
        private readonly IRaftConsensusPartitionTimerStore _raftConsensusPartitionTimerStore;
        private readonly IPartitionElectionStore _partitionElectionStore;
        private CancellationTokenSource _cancellationTokenSource;

        public RaftConsensusTimer(IRaftConsensusPartitionTimerStore raftConsensusPartitionTimerStore, IPartitionElectionStore partitionElectionStore)
        {
            _raftConsensusPartitionTimerStore = raftConsensusPartitionTimerStore;
            _partitionElectionStore = partitionElectionStore;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var partitions = await _partitionElectionStore.GetAll(cancellationToken);
            foreach (var partition in partitions)
            {
                var timer = _raftConsensusPartitionTimerStore.Add(partition, _cancellationTokenSource);
                await timer.Start();
            }
        }

        public void Stop()
        {
            var timers = _raftConsensusPartitionTimerStore.GetAll();
            foreach(var timer in timers) timer.Stop();
        }
    }
}
