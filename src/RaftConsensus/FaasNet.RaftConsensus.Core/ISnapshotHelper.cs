using FaasNet.RaftConsensus.Core.StateMachines;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public interface ISnapshotHelper
    {
        Task TakeSnapshot(long index);
        void EraseSnapshot(long index);
        void WriteSnapshot(long index, int recordIndex, IEnumerable<byte> buffer);
        IEnumerable<SnapshotChunkResult> ReadSnapshot(long index);
    }

    public class SnapshotHelper : ISnapshotHelper
    {
        private readonly RaftConsensusPeerOptions _raftOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISnapshotStore _snapshotStore;
        private PeerState _peerState;

        public SnapshotHelper(IOptions<RaftConsensusPeerOptions> raftOptions, IServiceProvider serviceProvider, ISnapshotStore snapshotStore)
        {
            _raftOptions = raftOptions.Value;
            _serviceProvider = serviceProvider;
            _snapshotStore = snapshotStore;
            _peerState = PeerState.New(_raftOptions.ConfigurationDirectoryPath, _raftOptions.IsConfigurationStoredInMemory);
        }

        public Task TakeSnapshot(long index)
        {
            var stateMachine = (IStateMachine)ActivatorUtilities.CreateInstance(_serviceProvider, _raftOptions.StateMachineType);
            _snapshotStore.Save(stateMachine, index);
            _peerState.SnapshotTerm = _peerState.CurrentTerm;
            _peerState.SnapshotLastApplied = _peerState.CommitIndex;
            _peerState.SnapshotCommitIndex = index;
            return Task.CompletedTask;
        }

        public void EraseSnapshot(long index)
        {
            _snapshotStore.Erase(index);
        }

        public void WriteSnapshot(long index, int recordIndex, IEnumerable<byte> buffer)
        {
            _snapshotStore.Write(index, recordIndex, buffer);
        }

        public IEnumerable<SnapshotChunkResult> ReadSnapshot(long index)
        {
            return _snapshotStore.Read(index);
        }
    }
}
