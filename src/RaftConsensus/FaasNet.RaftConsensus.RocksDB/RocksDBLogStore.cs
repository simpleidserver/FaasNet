using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
using RocksDbSharp;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.RocksDB
{
    public class RocksDBLogStore : ILogStore
    {
        private static SemaphoreSlim _lock = new SemaphoreSlim(1);
        private readonly RaftConsensusRocksDBOptions _options;
        private readonly RocksDBConnectionPool _connectionPool;

        public RocksDBLogStore(IOptions<RaftConsensusRocksDBOptions> options)
        {
            _options = options.Value;
            _connectionPool = new RocksDBConnectionPool();
        }

        public string TermId { get; set; }

        public void Add(LogRecord logRecord)
        {
            _lock.Wait();
            var versionDb = _connectionPool.GetConnection(BuildOptions(), GetDirectoryLogLatestVersion());
            var value = versionDb.Get(TermId);
            var version = 1;
            if (!string.IsNullOrWhiteSpace(value)) version = int.Parse(value) + 1;
            var historyDb = _connectionPool.GetConnection(BuildOptions(), GetDirectoryLogHistory(TermId));
            historyDb.Put(version.ToString(), JsonSerializer.Serialize(logRecord));
            versionDb.Put(TermId, version.ToString());
            _lock.Release();
        }

        public Task<LogRecord> Get(long index, CancellationToken cancellationToken)
        {
            var historyDb = _connectionPool.GetConnection(BuildOptions(), GetDirectoryLogHistory(TermId));
            var result = historyDb.Get(index.ToString());
            if (string.IsNullOrWhiteSpace(result)) return Task.FromResult((LogRecord)null);
            return Task.FromResult(JsonSerializer.Deserialize<LogRecord>(result, new JsonSerializerOptions {  PropertyNameCaseInsensitive = true }));
        }

        private string GetDirectoryLogHistory(string termId)
        {
            var result = Path.Combine(GetPath(), $"LogHistory{termId}");
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }

        private string GetDirectoryLogLatestVersion()
        {
            var result = Path.Combine(GetPath(), "LogLatestVersion");
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }

        private string GetPath()
        {
            if (string.IsNullOrWhiteSpace(_options.SubPath)) return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "storage");
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _options.SubPath, "storage");
        }

        private DbOptions BuildOptions()
        {
            var result = new DbOptions();
            _options.OptionsCallback(result);
            return result;
        }
    }
}
