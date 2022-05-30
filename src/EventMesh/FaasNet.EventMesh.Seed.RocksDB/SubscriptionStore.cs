using FaasNet.EventMesh.Seed.Stores;
using Microsoft.Extensions.Options;
using RocksDbSharp;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Seed.RocksDB
{
    public class SubscriptionStore : ISubscriptionStore
    {
        private static SemaphoreSlim _lock = new SemaphoreSlim(1);
        private RocksDb _rockDb;
        private readonly EventMeshSeedRocksDBOptions _options;

        public SubscriptionStore(IOptions<EventMeshSeedRocksDBOptions> options)
        {
            _options = options.Value;
        }

        public async Task<long> GetOffset(string jobId, string topic, CancellationToken cancellationToken)
        {
            await _lock.WaitAsync(cancellationToken);
            long result = 0;
            var connection = GetConnection();
            var value = connection.Get(BuildKey(jobId, topic));
            if (!string.IsNullOrWhiteSpace(value) && long.TryParse(value, out long v)) result = v;
            _lock.Release();
            return result;
        }

        public async Task IncrementOffset(string jobId, string topic, CancellationToken cancellationToken)
        {
            var offset = await GetOffset(jobId, topic, cancellationToken);
            await _lock.WaitAsync(cancellationToken);
            var connection = GetConnection();
            connection.Put(BuildKey(jobId, topic), (offset + 1).ToString());
            _lock.Release();
        }

        private DbOptions BuildOptions()
        {
            var result = new DbOptions();
            _options.OptionsCallback(result);
            return result;
        }

        private RocksDb GetConnection()
        {
            if(_rockDb == null)
            {
                _rockDb = RocksDb.Open(BuildOptions(), GetSubscriptionFolderPath());
            }

            return _rockDb;
        }

        private string GetSubscriptionFolderPath()
        {
            var result = Path.Combine(GetPath(), "Subscription");
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }

        private string GetPath()
        {
            if (string.IsNullOrWhiteSpace(_options.SubPath)) return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "storage");
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _options.SubPath, "storage");
        }

        private static string BuildKey(string jobId, string topic)
        {
            return $"{jobId}_{topic}";
        }
    }
}
