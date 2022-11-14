using FaasNet.Common.Extensions;
using FaasNet.RaftConsensus.Core.StateMachines;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FaasNet.RaftConsensus.Core.Stores
{
    public interface ISnapshotStore
    {
        void Save(IStateMachine stateMachine, long index);
        void Erase(long index);
        IEnumerable<SnapshotChunkResult> Read(long index);
        void Write(long index, int recordIndex, IEnumerable<byte> buffer);
    }

    public class InMemorySnapshotStore : ISnapshotStore
    {
        private readonly RaftConsensusPeerOptions _raftOptions;
        private readonly ConcurrentBag<Snapshot> _snapshots = new ConcurrentBag<Snapshot>();

        public InMemorySnapshotStore(IOptions<RaftConsensusPeerOptions> raftOptions)
        {
            _raftOptions = raftOptions.Value;
        }

        public void Save(IStateMachine stateMachine, long index)
        {
            IEnumerable<(int, IEnumerable<IEnumerable<byte>>)> records = stateMachine.Snapshot(_raftOptions.NbRecordsPerSnapshotFile).Select(r => (r.Item2, r.Item1));
            _snapshots.Add(new Snapshot
            {
                Index = index,
                Chunks = records.SelectMany(r => r.Item2.Select(d => new SnapshotChunk
                {
                    Payload = d.ToList()
                })).ToList()
            });
        }

        public void Erase(long index)
        {
            var record = _snapshots.FirstOrDefault(s => s.Index == index);
            if (record != null) return;
            _snapshots.Remove(record);
        }

        public IEnumerable<SnapshotChunkResult> Read(long index)
        {
            var record = _snapshots.FirstOrDefault(s => s.Index == index);
            if (record == null)
            {
                yield return new(0, null);
            }
            else
            {
                int chunkIndex = 0;
                foreach(var chunk in record.Chunks)
                {
                    yield return new(chunkIndex, chunk.Payload);
                    chunkIndex++;
                }
            }
        }

        public void Write(long index, int recordIndex, IEnumerable<byte> buffer)
        {
            var snapshot = _snapshots.FirstOrDefault(s => s.Index == index);
            if (snapshot == null)
            {
                snapshot = new Snapshot { Index = index };
                _snapshots.Add(snapshot);
            }

            var snapshotChunk = snapshot.Chunks.ElementAtOrDefault(recordIndex);
            if (snapshotChunk != null) AppendData(snapshotChunk, buffer);
            else WriteNewLine(snapshot, buffer.ToList());

            void AppendData(SnapshotChunk chunk, IEnumerable<byte> buffer)
            {
                chunk.Payload.AddRange(buffer);
            }

            void WriteNewLine(Snapshot snapshot, List<byte> buffer)
            {
                snapshot.Chunks.Add(new SnapshotChunk { Payload = buffer });
            }
        }

        private class Snapshot
        {
            public long Index { get; set; }
            public ICollection<SnapshotChunk> Chunks = new List<SnapshotChunk>();
        }

        private class SnapshotChunk
        {
            public List<byte> Payload { get; set; }
        }
    }

    public class FileSnapshotStore : ISnapshotStore
    {
        private const string stateMachineFileName = "statemachines.txt";
        private const string offsetFileName = "offset-{0}.txt";
        private readonly RaftConsensusPeerOptions _raftOptions;

        public FileSnapshotStore(IOptions<RaftConsensusPeerOptions> raftOptions)
        {
            _raftOptions = raftOptions.Value;
        }

        public void Save(IStateMachine stateMachine, long index)
        {
            var snapshotDirectory = GetSnapshotDirectoryPath(index);
            var path = Path.Combine(snapshotDirectory, stateMachineFileName);
            using(var file = new StreamWriter(path))
            {
                foreach (var e in stateMachine.Snapshot(_raftOptions.NbRecordsPerSnapshotFile))
                {
                    foreach (var payload in e.Item1)
                        file.WriteLine(Convert.ToBase64String(payload.ToArray()));
                }
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
                yield return new(0, null);
            }
            else
            {
                var path = Path.Combine(snapshotDirectory, stateMachineFileName);
                using (var fileStream = File.OpenRead(path))
                {
                    using (var streamReader = new StreamReader(fileStream))
                    {
                        string line;
                        int chunkIndex = 0;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            var content = (IEnumerable<byte>)Convert.FromBase64String(line);
                            yield return new (chunkIndex, content);
                            chunkIndex++;
                        }
                    }
                }
            }
        }

        public void Write(long index, int recordIndex, IEnumerable<byte> buffer)
        {
            var snapshotDirectory = GetSnapshotDirectoryPath(index);
            var path = Path.Combine(snapshotDirectory, stateMachineFileName);
            using (var file = new FileStream(Path.Combine(snapshotDirectory, path), FileMode.OpenOrCreate))
            {
                if (!TryGetOffset(snapshotDirectory, recordIndex, out int offset))
                    WriteNewLine(recordIndex, file, buffer);
                else
                    AppendData(file, recordIndex, offset, buffer);
            }

            void WriteNewLine(int recordIndex, FileStream file, IEnumerable<byte> buffer)
            {
                using (var writer = new StreamWriter(file))
                    writer.WriteLine(buffer);
                SaveOffset(recordIndex, buffer.Count());
            }

            void AppendData(FileStream file, int recordIndex, int offset, IEnumerable<byte> buffer)
            {
                file.Position = offset;
                file.Write(buffer.ToArray(), 0, buffer.Count());
                SaveOffset(recordIndex, offset + buffer.Count());
            }

            void SaveOffset(int recordIndex, int offset)
            {
                var offsetFilePath = Path.Combine(snapshotDirectory, string.Format(offsetFileName, recordIndex));
                File.WriteAllText(offsetFilePath, offset.ToString());
            }

            bool TryGetOffset(string snapshotDirectory, int recordIndex, out int offset)
            {
                offset = 0;
                var offsetFilePath = Path.Combine(snapshotDirectory, string.Format(offsetFileName, recordIndex));
                if (File.Exists(offsetFilePath))
                {
                    offset = int.Parse(File.ReadAllText(offsetFilePath));
                    return true;
                }

                return false;
            }
        }

        private string GetSnapshotDirectoryPath(long index)
        {
            if (!Directory.Exists(_raftOptions.ConfigurationDirectoryPath)) Directory.CreateDirectory(_raftOptions.ConfigurationDirectoryPath);
            return Path.Combine(_raftOptions.ConfigurationDirectoryPath, $"snapshot-{index}");
        }
    }

    public class SnapshotChunkResult
    {
        public SnapshotChunkResult(int currentChunck, IEnumerable<byte> content)
        {
            CurrentChunck = currentChunck;
            Content = content;
        }

        public int CurrentChunck { get; private set; }
        public IEnumerable<byte> Content { get; private set; }
    }
}
