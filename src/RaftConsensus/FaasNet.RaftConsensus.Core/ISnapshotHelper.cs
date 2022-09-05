using FaasNet.Common.Extensions;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Client.StateMachines;
using FaasNet.RaftConsensus.Core.StateMachines;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public interface ISnapshotHelper
    {
        IEnumerable<(IStateMachine, Snapshot)> RestoreAllStateMachines(IEnumerable<LogEntry> logEntries);
        IEnumerable<(IStateMachine, Snapshot)> GetAllStateMachines();
        Task<IEnumerable<(IStateMachine, Snapshot)>> GetAllLatestStateMachines(CancellationToken cancellationToken);
        Task<(IStateMachine, Snapshot)> GetLatestStateMachine(string stateMachine, CancellationToken cancellationToken);
    }

    public class SnapshotHelper : ISnapshotHelper
    {
        private readonly ILogStore _logStore;
        private readonly ISnapshotStore _snapshotStore;
        private readonly RaftConsensusPeerOptions _options;
        private PeerState _peerState;

        public SnapshotHelper(ILogStore logStore, ISnapshotStore snapshotStore, IOptions<RaftConsensusPeerOptions> options)
        {
            _logStore = logStore;
            _snapshotStore = snapshotStore;
            _options = options.Value;
            _peerState = PeerState.New(_options.ConfigurationDirectoryPath, _options.IsConfigurationStoredInMemory);
        }

        public IEnumerable<(IStateMachine, Snapshot)> RestoreAllStateMachines(IEnumerable<LogEntry> logEntries)
        {
            var result = new ConcurrentBag<(IStateMachine, Snapshot)>();
            var missingStateMachineIds = new ConcurrentBag<string>(logEntries.Select(e => e.StateMachineId).Distinct());
            Restore(result, missingStateMachineIds);
            BuildNew(result, missingStateMachineIds);
            return result.ToList();

            void Restore(ConcurrentBag<(IStateMachine, Snapshot)> result, ConcurrentBag<string> missingStateMachineIds)
            {
                Parallel.ForEach(_snapshotStore.GetAll(), new ParallelOptions
                {
                    MaxDegreeOfParallelism = _options.MaxNbThreads
                }, (s, t) =>
                {
                    var stateMachine = StateMachineSerializer.Deserialize(_options.StateMachineType, s.StateMachine);
                    stateMachine.Id = s.StateMachineId;
                    var filteredLogEntries = logEntries.Where(le => le.StateMachineId == s.StateMachineId && le.Index > s.Index).OrderBy(l => l.Index);
                    if (!filteredLogEntries.Any())
                    {
                        result.Add((stateMachine, s));
                        return;
                    }

                    foreach (var logEntry in filteredLogEntries.OrderBy(l => l.Index))
                    {
                        missingStateMachineIds.Remove(stateMachine.Id);
                        var cmd = CommandSerializer.Deserialize(logEntry.Command);
                        stateMachine.Apply(cmd);
                    }

                    var snapshot = new Snapshot
                    {
                        Index = _peerState.CommitIndex,
                        StateMachine = stateMachine.Serialize(),
                        StateMachineId = s.StateMachineId,
                        Term = filteredLogEntries.OrderByDescending(l => l.Term).First().Term
                    };
                    result.Add((stateMachine, snapshot));
                });
            }

            void BuildNew(ConcurrentBag<(IStateMachine, Snapshot)> result, ConcurrentBag<string> missingStateMachineIds)
            {
                Parallel.ForEach(missingStateMachineIds, new ParallelOptions
                {
                    MaxDegreeOfParallelism = _options.MaxNbThreads
                }, (s, t) =>
                {
                    var filteredLogEntries = logEntries.Where(le => le.StateMachineId == s).OrderBy(l => l.Index);
                    var stateMachine = (IStateMachine)Activator.CreateInstance(_options.StateMachineType);
                    stateMachine.Id = s;
                    foreach (var logEntry in filteredLogEntries.OrderBy(l => l.Index))
                    {
                        var cmd = CommandSerializer.Deserialize(logEntry.Command);
                        stateMachine.Apply(cmd);
                    }

                    var snapshot = new Snapshot
                    {
                        Index = _peerState.CommitIndex,
                        StateMachineId = s,
                        StateMachine = stateMachine.Serialize(),
                        Term = filteredLogEntries.OrderByDescending(l => l.Term).First().Term
                    };
                    result.Add((stateMachine, snapshot));
                });
            }
        }

        public IEnumerable<(IStateMachine, Snapshot)> GetAllStateMachines()
        {
            var result = new ConcurrentBag<(IStateMachine, Snapshot)>();
            Restore(result);
            return result;

            void Restore(ConcurrentBag<(IStateMachine, Snapshot)> result)
            {
                Parallel.ForEach(_snapshotStore.GetAll(), new ParallelOptions
                {
                    MaxDegreeOfParallelism = _options.MaxNbThreads
                }, (s, t) =>
                {
                    var stateMachine = StateMachineSerializer.Deserialize(_options.StateMachineType, s.StateMachine);
                    stateMachine.Id = s.StateMachineId;
                    result.Add((stateMachine, s));
                });
            }
        }

        public async Task<IEnumerable<(IStateMachine, Snapshot)>> GetAllLatestStateMachines(CancellationToken cancellationToken)
        {
            var result = new ConcurrentBag<(IStateMachine, Snapshot)>();
            await Restore(result);
            return result;


            async Task Restore(ConcurrentBag<(IStateMachine, Snapshot)> result)
            {
                await Parallel.ForEachAsync(_snapshotStore.GetAll(), new ParallelOptions
                {
                    MaxDegreeOfParallelism = _options.MaxNbThreads
                }, async (s, t) =>
                {
                    var stateMachine = StateMachineSerializer.Deserialize(_options.StateMachineType, s.StateMachine);
                    stateMachine.Id = s.StateMachineId;
                    var index = s.Index;
                    var logEntries = await _logStore.GetFrom(index, false, cancellationToken);
                    foreach (var logEntry in logEntries)
                    {
                        var cmd = CommandSerializer.Deserialize(logEntry.Command);
                        stateMachine.Apply(cmd);
                    }

                    result.Add((stateMachine, s));
                });
            }
        }

        public async Task<(IStateMachine, Snapshot)> GetLatestStateMachine(string stateMachineId, CancellationToken cancellationToken)
        {
            var snapshot = _snapshotStore.Get(stateMachineId);
            var index = snapshot == null ? 0 : snapshot.Index;
            IStateMachine stateMachine = null;
            if (snapshot != null)
            {
                stateMachine = StateMachineSerializer.Deserialize(_options.StateMachineType, snapshot.StateMachine);
                stateMachine.Id = stateMachineId;
            }
            else
                stateMachine = (IStateMachine)Activator.CreateInstance(_options.StateMachineType);

            var logEntries = await _logStore.GetFrom(index, false, cancellationToken);
            foreach (var logEntry in logEntries)
            {
                stateMachine.Id = stateMachineId;
                var cmd = CommandSerializer.Deserialize(logEntry.Command);
                stateMachine.Apply(cmd);
            }

            return (stateMachine, snapshot ?? new Snapshot());
        }
    }
}
