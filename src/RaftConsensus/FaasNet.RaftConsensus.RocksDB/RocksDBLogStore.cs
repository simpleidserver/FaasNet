using FaasNet.RaftConsensus.Client;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
using RocksDbSharp;
using System;
using System.Collections.Generic;
using System.IO;
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
            var versionDb = _connectionPool.GetConnection(BuildOptions(), GetDirectoryPath());
            versionDb.Put(BitConverter.GetBytes(entry.Index), entry.Serialize());
            return Task.CompletedTask;
        }

        public Task<LogEntry> Get(long index, CancellationToken cancellationToken)
        {
            LogEntry result = null;
            var versionDb = _connectionPool.GetConnection(BuildOptions(), GetDirectoryPath());
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
            var versionDb = _connectionPool.GetConnection(BuildOptions(), GetDirectoryPath());
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

        public Task<IEnumerable<LogEntry>> GetTo(long index, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveFrom(long startIndex, CancellationToken cancellation)
        {
            var logEntries = await GetFrom(startIndex, cancellationToken: cancellation);
            var versionDb = _connectionPool.GetConnection(BuildOptions(), GetDirectoryPath());
            foreach(var logEntry in logEntries) versionDb.Remove(BitConverter.GetBytes(logEntry.Index));
        }

        public Task RemoveTo(long endIndex, CancellationToken cancellation)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateRange(IEnumerable<LogEntry> entries, CancellationToken cancellationToken)
        {
            foreach (var entry in entries) await Append(entry, cancellationToken);
        }

        private DbOptions BuildOptions()
        {
            var result = new DbOptions();
            _rockDbOptions.OptionsCallback(result);
            return result;
        }

        private string GetDirectoryPath()
        {
            if (!Directory.Exists(_raftOptions.ConfigurationDirectoryPath)) Directory.CreateDirectory(_raftOptions.ConfigurationDirectoryPath);
            var result = Path.Combine(_raftOptions.ConfigurationDirectoryPath, "logs");
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }
    }
}
