using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core.StateMachines;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface ISnapshotStore
    {
        IStateMachine RestoreStateMachine(IEnumerable<LogEntry> logEntries);
        Task<(IStateMachine, Snapshot)> GetLatestStateMachine(CancellationToken cancellationToken);
        void Update(Snapshot snapshot);
        Task<Snapshot> Get(CancellationToken cancellationToken);
    }

    public class InMemorySnapshotStore : ISnapshotStore
    {
        private readonly ILogStore _logStore;
        private readonly RaftConsensusPeerOptions _options;
        private Snapshot _snapshot = null;

        public InMemorySnapshotStore(ILogStore logStore, IOptions<RaftConsensusPeerOptions> options)
        {
            _logStore = logStore;
            _options = options.Value;
        }

        public IStateMachine RestoreStateMachine(IEnumerable<LogEntry> logEntries)
        {
            IStateMachine stateMachine;
            if (_snapshot != null)
                stateMachine = StateMachineSerializer.Deserialize(_options.StateMachineType, _snapshot.StateMachine);
            else
                stateMachine = (IStateMachine)Activator.CreateInstance(_options.StateMachineType);
            foreach(var logEntry in logEntries.OrderBy(l => l.Index))
            {
                var cmd = CommandSerializer.Deserialize(logEntry.Command);
                stateMachine.Apply(cmd);
            }

            return stateMachine;
        }

        public async Task<(IStateMachine, Snapshot)> GetLatestStateMachine(CancellationToken cancellationToken)
        {
            IStateMachine stateMachine;
            var index = _snapshot == null ? 0 : _snapshot.Index;
            if (_snapshot != null)
                stateMachine = StateMachineSerializer.Deserialize(_options.StateMachineType, _snapshot.StateMachine);
            else
                stateMachine = (IStateMachine)Activator.CreateInstance(_options.StateMachineType);

            var logEntries = await _logStore.GetFrom(index, false, cancellationToken);
            foreach(var logEntry in logEntries)
            {
                var cmd = CommandSerializer.Deserialize(logEntry.Command);
                stateMachine.Apply(cmd);
            }

            return (stateMachine, _snapshot ?? new Snapshot());
        }

        public void Update(Snapshot snapshot)
        {
            _snapshot = snapshot;
        }

        public Task<Snapshot> Get(CancellationToken cancellationToken)
        {
            return Task.FromResult(_snapshot);
        }
    }

    public class Snapshot
    {
        public long Index { get; set; }
        public long Term { get; set; }
        public byte[] StateMachine { get; set; }
    }
}
