using FaasNet.RaftConsensus.Core.StateMachines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.Core
{
    public interface ISnapshotHelper
    {
        Task TakeSnapshot(long index);
        void EraseSnapshot(long index);
        void WriteSnapshot(long index, int iteration, IEnumerable<IEnumerable<byte>> buffer);
        IEnumerable<(IEnumerable<IEnumerable<byte>>, int, int)> ReadSnapshot(long index);
    }

    public class SnapshotHelper : ISnapshotHelper
    {
        private readonly RaftConsensusPeerOptions _raftOptions;
        private readonly IServiceProvider _serviceProvider;
        private PeerState _peerState;

        public SnapshotHelper(IOptions<RaftConsensusPeerOptions> raftOptions, IServiceProvider serviceProvider)
        {
            _raftOptions = raftOptions.Value;
            _serviceProvider = serviceProvider;
            _peerState = PeerState.New(_raftOptions.ConfigurationDirectoryPath, _raftOptions.IsConfigurationStoredInMemory);
        }

        public Task TakeSnapshot(long index)
        {
            var snapshotDirectory = GetSnapshotDirectoryPath(index);
            Directory.CreateDirectory(snapshotDirectory);
            var stateMachine = (IStateMachine)ActivatorUtilities.CreateInstance(_serviceProvider, _raftOptions.StateMachineType);
            foreach(var e in stateMachine.Snapshot(_raftOptions.NbRecordsPerSnapshotFile))
            {
                var path = Path.Combine(snapshotDirectory, $"statemachines-{e.Item2}.txt");
                using (var file = new StreamWriter(path))
                    foreach (var payload in e.Item1)
                        file.WriteLine(Convert.ToBase64String(payload.ToArray()));
            }

            _peerState.SnapshotTerm = _peerState.CurrentTerm;
            _peerState.SnapshotLastApplied = _peerState.CommitIndex;
            return Task.CompletedTask;
        }

        public void EraseSnapshot(long index)
        {
            var directoryPath = GetSnapshotDirectoryPath(index);
            if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath, true);
        }

        public void WriteSnapshot(long index, int iteration, IEnumerable<IEnumerable<byte>> buffer)
        {
            var directoryPath = GetSnapshotDirectoryPath(index);
            if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath);
            Directory.CreateDirectory(directoryPath);
            using (var file = new StreamWriter(Path.Combine(directoryPath, $"statemachines-{iteration}.txt")))
                foreach (var payload in buffer)
                    file.WriteLine(Convert.ToBase64String(payload.ToArray()));
            
        }

        public IEnumerable<(IEnumerable<IEnumerable<byte>>, int, int)> ReadSnapshot(long index)
        {
            var snapshotDirectory = GetSnapshotDirectoryPath(index);
            if (!Directory.Exists(snapshotDirectory))
            {
                yield return new(null, 0, 0);
            }
            else
            {
                var files = Directory.GetFiles(snapshotDirectory, "*.txt");
                foreach (var file in files.OrderBy(f => f))
                {
                    var number = int.Parse(Path.GetFileName(file).Replace(".txt", string.Empty).Split('-').Last());
                    yield return (File.ReadAllLines(file).Select(l => (IEnumerable<byte>)Convert.FromBase64String(l)), number, files.Count());
                }
            }
        }

        private string GetSnapshotDirectoryPath(long index)
        {
            if (!Directory.Exists(_raftOptions.ConfigurationDirectoryPath)) Directory.CreateDirectory(_raftOptions.ConfigurationDirectoryPath);
            return Path.Combine(_raftOptions.ConfigurationDirectoryPath, $"snapshot-{index}");
        }
    }
}
