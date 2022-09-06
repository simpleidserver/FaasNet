using FaasNet.Peer.Client;
using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
using RocksDbSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.RocksDB
{
    public class RocksDBLogStore : ILogStore
    {
        private readonly RaftConsensusRocksDBOptions _rockDbOptions;
        private readonly RaftConsensusPeerOptions _raftOptions;
        private readonly RocksDBConnectionPool _connectionPool;

        public RocksDBLogStore(IOptions<RaftConsensusRocksDBOptions> rockDbOptions, IOptions<RaftConsensusPeerOptions> raftOptions)
        {
            _rockDbOptions = rockDbOptions.Value;
            _raftOptions = raftOptions.Value;
            _connectionPool = new RocksDBConnectionPool();
        }

        public Task Append(LogEntry entry, CancellationToken cancellationToken)
        {
            var versionDb = _connectionPool.GetConnection(BuildOptions(), GetLogsDirectoryPath());
            versionDb.Put(BitConverter.GetBytes(entry.Index), entry.Serialize());
            StoreStateMachineId();
            return Task.CompletedTask;

            void StoreStateMachineId()
            {
                var stateMachineIdDb = _connectionPool.GetConnection(BuildOptions(), GetStateMachineIdDirectoryPath());
                var stateMachineIdPayload = Encoding.UTF8.GetBytes(entry.StateMachineId);
                if (stateMachineIdDb.Get(stateMachineIdPayload) == null)
                    stateMachineIdDb.Put(stateMachineIdPayload, BitConverter.GetBytes(entry.Index));
            }
        }

        public Task<LogEntry> Get(long index, CancellationToken cancellationToken)
        {
            LogEntry result = null;
            var versionDb = _connectionPool.GetConnection(BuildOptions(), GetLogsDirectoryPath());
            var payload = versionDb.Get(BitConverter.GetBytes(index));
            if (payload != null) result = LogEntry.Deserialize(payload);
            return Task.FromResult(result);
        }

        public async Task<LogEntry> Get(long term, long index, CancellationToken cancellationToken)
        {
            var result = await Get(index, cancellationToken);
            if (result == null || result.Term != term) return null;
            return result;
        }

        public Task<IEnumerable<LogEntry>> GetFrom(long startIndex, bool equal = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<LogEntry>();
            var versionDb = _connectionPool.GetConnection(BuildOptions(), GetLogsDirectoryPath());
            using (var iterator = versionDb.NewIterator())
            {
                iterator.Seek(BitConverter.GetBytes(startIndex));
                while(iterator.Valid())
                {
                    var payload = iterator.Value();
                    if(payload != null) result.Add(LogEntry.Deserialize(payload));
                    iterator.Next();
                }
            }

            return Task.FromResult((IEnumerable<LogEntry>)result);
        }

        public Task<IEnumerable<LogEntry>> GetFromTo(long fromIndex, long toIndex, CancellationToken cancellationToken)
        {
            var result = new List<LogEntry>();
            var versionDb = _connectionPool.GetConnection(BuildOptions(), GetLogsDirectoryPath());
            using (var iterator = versionDb.NewIterator())
            {
                iterator.Seek(BitConverter.GetBytes(fromIndex));
                while (iterator.Valid())
                {
                    var key = iterator.Key();
                    var payload = iterator.Value();
                    if (payload != null) result.Add(LogEntry.Deserialize(payload));
                    iterator.Next();
                    if (BitConverter.ToInt64(key) == toIndex) break; 
                }
            }

            return Task.FromResult((IEnumerable<LogEntry>)result);
        }

        public async Task RemoveFrom(long startIndex, CancellationToken cancellation)
        {
            var logEntries = await GetFrom(startIndex, cancellationToken: cancellation);
            var versionDb = _connectionPool.GetConnection(BuildOptions(), GetLogsDirectoryPath());
            foreach(var logEntry in logEntries) versionDb.Remove(BitConverter.GetBytes(logEntry.Index));
            var unusedStateMachineIds = GetUnusedStateMachineIds();
            RemoveUnusedStateMachineIds(unusedStateMachineIds);

            IEnumerable<string> GetUnusedStateMachineIds()
            {
                var result = new List<string>();
                var stateMachineDb = _connectionPool.GetConnection(BuildOptions(), GetStateMachineIdDirectoryPath());
                using (var iterator = stateMachineDb.NewIterator())
                {
                    while (iterator.Valid())
                    {
                        var value = iterator.Value();
                        if (value != null)
                        {
                            var key = iterator.Key();
                            var index = BitConverter.ToInt64(value);
                            if (index >= startIndex) result.Add(Encoding.UTF8.GetString(key));
                        }
                    }
                }

                return result;
            }

            void RemoveUnusedStateMachineIds(IEnumerable<string> stateMachineIds)
            {
                var stateMachineDb = _connectionPool.GetConnection(BuildOptions(), GetStateMachineIdDirectoryPath());
                foreach (var stateMachineId in stateMachineIds) stateMachineDb.Remove(stateMachineId);
            }
        }

        public async Task UpdateRange(IEnumerable<LogEntry> entries, CancellationToken cancellationToken)
        {
            foreach (var entry in entries) await Append(entry, cancellationToken);
        }

        public Task<string> GetStateMachineId(int index, CancellationToken cancellationToken)
        {
            int currentIndex = 0;
            var stateMachineDb = _connectionPool.GetConnection(BuildOptions(), GetStateMachineIdDirectoryPath());
            using (var iterator = stateMachineDb.NewIterator())
            {
                while (iterator.Valid())
                {
                    var key = iterator.Key();
                    if (currentIndex == index)
                    {
                        if(key == null) return Task.FromResult((string)null);
                        return Task.FromResult(Encoding.UTF8.GetString(key));
                    }

                    currentIndex++;
                }
            }

            return Task.FromResult((string)null);
        }

        private DbOptions BuildOptions()
        {
            var result = new DbOptions();
            _rockDbOptions.OptionsCallback(result);
            return result;
        }

        private string GetLogsDirectoryPath()
        {
            if (!Directory.Exists(_raftOptions.ConfigurationDirectoryPath)) Directory.CreateDirectory(_raftOptions.ConfigurationDirectoryPath);
            var result = Path.Combine(_raftOptions.ConfigurationDirectoryPath, "logs");
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }

        private string GetStateMachineIdDirectoryPath()
        {
            if (!Directory.Exists(_raftOptions.ConfigurationDirectoryPath)) Directory.CreateDirectory(_raftOptions.ConfigurationDirectoryPath);
            var result = Path.Combine(_raftOptions.ConfigurationDirectoryPath, "statemachinesid");
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }

        private class StateMachineRecord
        {
            public string StateMachineId { get; set; }
            public long LogIndex { get; set; }

            public byte[] Serialize()
            {
                var context = new WriteBufferContext();
                context.WriteString(StateMachineId);
                context.WriteLong(LogIndex);
                return context.Buffer.ToArray();
            }

            public static StateMachineRecord Deserialize(byte[] payload)
            {
                var context = new ReadBufferContext(payload);
                return new StateMachineRecord
                {
                    StateMachineId = context.NextString(),
                    LogIndex = context.NextLong()
                };
            }
        }
    }
}
