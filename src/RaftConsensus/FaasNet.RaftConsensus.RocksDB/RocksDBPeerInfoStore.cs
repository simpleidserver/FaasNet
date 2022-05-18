using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
using RocksDbSharp;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.RocksDB
{
    public class RocksDBPeerInfoStore : IPeerInfoStore
    {
        private static SemaphoreSlim _lock = new SemaphoreSlim(1);
        private readonly RaftConsensusRocksDBOptions _options;
        private readonly RocksDBConnectionPool _connectionPool;

        public RocksDBPeerInfoStore(IOptions<RaftConsensusRocksDBOptions> options)
        {
            _options = options.Value;
            _connectionPool = new RocksDBConnectionPool();
        }

        public void Add(PeerInfo peerInfo)
        {
            _lock.Wait();
            var db = _connectionPool.GetConnection(BuildOptions(), GetFolderPeerInfo());
            var json = JsonSerializer.Serialize(peerInfo);
            db.Put(peerInfo.TermId, json);
            _lock.Release();
        }

        public void Update(PeerInfo peerInfo)
        {
            Add(peerInfo);
        }

        public async Task<IEnumerable<PeerInfo>> GetAll(CancellationToken cancellationToken)
        {
            _lock.Wait();
            var result = new List<PeerInfo>();
            var db = _connectionPool.GetConnection(BuildOptions(), GetFolderPeerInfo());
            using (var iterator = db.NewIterator())
            {
                iterator.SeekToFirst();
                while (iterator.Valid())
                {
                    var value = Encoding.UTF8.GetString(iterator.Value());
                    result.Add(await Parse(value));
                    iterator.Next();
                }
            }

            _lock.Release();
            return result;
        }

        private static Task<PeerInfo> Parse(string value)
        {
            if (value == null) return Task.FromResult((PeerInfo)null);
            return Task.FromResult(JsonSerializer.Deserialize<PeerInfo>(value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
        }

        private DbOptions BuildOptions()
        {
            var result = new DbOptions();
            _options.OptionsCallback(result);
            return result;
        }

        private string GetFolderPeerInfo()
        {
            var result = Path.Combine(GetPath(), "PeerInfo");
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }

        private string GetPath()
        {
            if (string.IsNullOrWhiteSpace(_options.SubPath)) return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "storage");
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _options.SubPath, "storage");
        }
    }
}
