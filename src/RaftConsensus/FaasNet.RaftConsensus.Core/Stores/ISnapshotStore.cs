using FaasNet.RaftConsensus.Core.StateMachines;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface ISnapshotStore
    {
        void Save(IStateMachine stateMachine, long index);
        void Erase(long index);
        IEnumerable<SnapshotChunkResult> Read(long index);
        void Write(long index, int iteration, IEnumerable<IEnumerable<byte>> buffer);
    }

    public class InMemorySnapshotStore : ISnapshotStore
    {
        private readonly RaftConsensusPeerOptions _raftOptions;
        private readonly ConcurrentDictionary<long, IEnumerable<(int, IEnumerable<IEnumerable<byte>>)>> _snapshot = new ConcurrentDictionary<long, IEnumerable<(int, IEnumerable<IEnumerable<byte>>)>>();

        public InMemorySnapshotStore(IOptions<RaftConsensusPeerOptions> raftOptions)
        {
            _raftOptions = raftOptions.Value;
        }

        public void Save(IStateMachine stateMachine, long index)
        {
            IEnumerable<(int, IEnumerable<IEnumerable<byte>>)> records = stateMachine.Snapshot(_raftOptions.NbRecordsPerSnapshotFile).Select(r => (r.Item2, r.Item1));
            _snapshot.AddOrUpdate(index, records, (k, o) => records);
        }

        public void Erase(long index)
        {
            _snapshot.Remove(index, out IEnumerable<(int, IEnumerable<IEnumerable<byte>>)> r);
        }

        public IEnumerable<SnapshotChunkResult> Read(long index)
        {
            IEnumerable<(int, IEnumerable<IEnumerable<byte>>)> res;
            if (!_snapshot.TryGetValue(index, out res))
            {
                yield return new(0, 0, null);
            }
            else
            {
                var nbChuncks = res.Count();
                foreach (var record in res.OrderBy(r => r.Item1))
                    yield return new(nbChuncks, record.Item1, record.Item2);
            }
        }

        public void Write(long index, int iteration, IEnumerable<IEnumerable<byte>> buffer)
        {
            IEnumerable<(int, IEnumerable<IEnumerable<byte>>)> empty = new List<(int, IEnumerable<IEnumerable<byte>>)>();
            if (!_snapshot.ContainsKey(index)) _snapshot.AddOrUpdate(index, empty, (l, o) => empty);
            var lst = _snapshot[index].ToList();
            lst.Add((iteration, buffer));
            _snapshot[index] = lst;
        }
    }

    public class FileSnapshotStore : ISnapshotStore
    {
        private readonly RaftConsensusPeerOptions _raftOptions;

        public FileSnapshotStore(IOptions<RaftConsensusPeerOptions> raftOptions)
        {
            _raftOptions = raftOptions.Value;
        }

        public void Save(IStateMachine stateMachine, long index)
        {
            var snapshotDirectory = GetSnapshotDirectoryPath(index);
            foreach (var e in stateMachine.Snapshot(_raftOptions.NbRecordsPerSnapshotFile))
            {
                var path = Path.Combine(snapshotDirectory, $"statemachines-{e.Item2}.txt");
                using (var file = new StreamWriter(path))
                    foreach (var payload in e.Item1)
                        file.WriteLine(Convert.ToBase64String(payload.ToArray()));
            }
        }

        public void Erase(long index)
        {
            var directoryPath = GetSnapshotDirectoryPath(index);
            if (Directory.Exists(directoryPath)) Directory.Delete(directoryPath, true);
        }

        public IEnumerable<SnapshotChunkResult> Read(long index)
        {
            var snapshotDirectory = GetSnapshotDirectoryPath(index);
            if (!Directory.Exists(snapshotDirectory))
            {
                yield return new(0, 0, null);
            }
            else
            {
                var chuncks = Directory.GetFiles(snapshotDirectory, "*.txt");
                var nbChuncks = chuncks.Count();
                foreach (var file in chuncks.OrderBy(f => f))
                {
                    var number = int.Parse(Path.GetFileName(file).Replace(".txt", string.Empty).Split('-').Last());
                    var content = File.ReadAllLines(file).Select(l => (IEnumerable<byte>)Convert.FromBase64String(l));
                    yield return new SnapshotChunkResult(nbChuncks, number, content);
                }
            }
        }

        public void Write(long index, int iteration, IEnumerable<IEnumerable<byte>> buffer)
        {
            var snapshotDirectory = GetSnapshotDirectoryPath(index);
            Directory.CreateDirectory(snapshotDirectory);
            using (var file = new StreamWriter(Path.Combine(snapshotDirectory, $"statemachines-{iteration}.txt")))
                foreach (var payload in buffer)
                    file.WriteLine(Convert.ToBase64String(payload.ToArray()));
        }

        private string GetSnapshotDirectoryPath(long index)
        {
            if (!Directory.Exists(_raftOptions.ConfigurationDirectoryPath)) Directory.CreateDirectory(_raftOptions.ConfigurationDirectoryPath);
            return Path.Combine(_raftOptions.ConfigurationDirectoryPath, $"snapshot-{index}");
        }
    }

    public class SnapshotChunkResult
    {
        public SnapshotChunkResult(int nbChuncks, int currentChunck, IEnumerable<IEnumerable<byte>> content)
        {
            NbChuncks = nbChuncks;
            CurrentChunck = currentChunck;
            Content = content;
        }

        public int NbChuncks { get; private set; }
        public int CurrentChunck { get; private set; }
        public IEnumerable<IEnumerable<byte>> Content { get; private set; }
    }
}
