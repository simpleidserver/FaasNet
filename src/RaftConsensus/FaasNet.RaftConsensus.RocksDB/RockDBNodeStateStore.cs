using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
using RocksDbSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.RaftConsensus.RocksDB
{
    public class RockDBNodeStateStore : INodeStateStore
    {
        private static SemaphoreSlim _lock = new SemaphoreSlim(1);
        private readonly RaftConsensusRocksDBOptions _options;

        public RockDBNodeStateStore(IOptions<RaftConsensusRocksDBOptions> options)
        {
            _options = options.Value;
        }

        public void Add(NodeState nodeState)
        {
            _lock.Wait();
            var json = JsonSerializer.Serialize(nodeState);
            StoreEntityId(nodeState, json);
            StoreEntityType(nodeState, json);
            _lock.Release();
        }

        public void Update(NodeState nodeState)
        {
            Add(nodeState);
        }

        public async Task<IEnumerable<NodeState>> GetAllLastEntityTypes(CancellationToken cancellationToken)
        {
            _lock.Wait();
            var result = new List<NodeState>();
            using (var db = RocksDb.Open(BuildOptions(), GetFolderPathByTypeValue()))
            {
                var iterator = db.NewIterator();
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

        public async Task<IEnumerable<NodeState>> GetAllLastEntityTypes(string entityType, CancellationToken cancellationToken)
        {
            _lock.Wait();
            var result = new List<NodeState>();
            using (var db = RocksDb.Open(BuildOptions(), GetFolderPathByTypeVersion(entityType)))
            {
                var iterator = db.NewIterator();
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

        public async Task<NodeState> GetSpecificEntityType(string entityType, int entityVersion, CancellationToken cancellationToken)
        {
            var entityTypes = await GetAllSpecificEntityTypes(new List<(string EntityType, int EntityVersion)> { (EntityType: entityType, EntityVersion: entityVersion) }, cancellationToken);
            return entityTypes.FirstOrDefault();
        }

        public Task<IEnumerable<NodeState>> GetAllSpecificEntityTypes(List<(string EntityType, int EntityVersion)> parameter, CancellationToken cancellationToken)
        {
            _lock.Wait();
            var result = new List<NodeState>();
            foreach(var grp in parameter.GroupBy(k => k.EntityType))
            {
                using (var db = RocksDb.Open(BuildOptions(), GetFolderPathByHistoryTypeVersionValue(grp.Key)))
                {
                    foreach (var record in grp)
                    {
                        var value = db.Get(record.EntityVersion.ToString());
                        if (string.IsNullOrWhiteSpace(value)) continue;
                        result.Add(JsonSerializer.Deserialize<NodeState>(value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
                    }
                }
            }

            _lock.Release();
            return Task.FromResult((IEnumerable<NodeState>)result);
        }

        public Task<NodeState> GetLastEntityId(string entityId, CancellationToken cancellationToken)
        {
            _lock.Wait();
            using (var db = RocksDb.Open(BuildOptions(), GetFolderPathByLastIdValue()))
            {
                var value = db.Get(entityId);
                _lock.Release();
                return Parse(value);
            }
        }

        public Task<NodeState> GetLastEntityType(string entityType, CancellationToken cancellationToken)
        {
            _lock.Wait();
            using (var db = RocksDb.Open(BuildOptions(), GetFolderPathByTypeValue()))
            {
                var value = db.Get(entityType);
                _lock.Release();
                return Parse(value);
            }
        }

        private DbOptions BuildOptions()
        {
            var result = new DbOptions();
            _options.OptionsCallback(result);
            return result;
        }

        private static Task<NodeState> Parse(string value)
        {
            if (value == null) return Task.FromResult((NodeState)null);
            return Task.FromResult(JsonSerializer.Deserialize<NodeState>(value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
        }

        private void StoreEntityId(NodeState nodeState, string json)
        {
            using (var db = RocksDb.Open(BuildOptions(), GetFolderPathByLastIdValue()))
            {
                db.Put(nodeState.EntityId, json);
            }

            using (var db = RocksDb.Open(BuildOptions(), GetFolderPathByLastVersionValue()))
            {
                db.Put(nodeState.EntityId, nodeState.EntityVersion.ToString());
            }
        }

        private void StoreEntityType(NodeState nodeState, string json)
        {
            using (var db = RocksDb.Open(BuildOptions(), GetFolderPathByTypeValue()))
            {
                db.Put(nodeState.EntityType, json);
            }

            using (var db = RocksDb.Open(BuildOptions(), GetFolderPathByHistoryTypeVersionValue(nodeState.EntityType)))
            {
                db.Put(nodeState.EntityVersion.ToString(), json);
            }

            using (var db = RocksDb.Open(BuildOptions(), GetFolderPathByTypeVersion(nodeState.EntityType)))
            {
                db.Put(nodeState.EntityId, json);
            }
        }

        private string GetFolderPathByLastIdValue()
        {
            var result = Path.Combine(GetPath(), "Ids");
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }

        private string GetFolderPathByLastVersionValue()
        {
            var result = Path.Combine(GetPath(), "Versions");
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }

        private string GetFolderPathByTypeValue()
        {
            var result = Path.Combine(GetPath(), "TypeVersions");
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }

        private string GetFolderPathByHistoryTypeVersionValue(string type)
        {
            var result = Path.Combine(GetPath(), $"HistoryTypeVersions{type}");
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }

        private string GetFolderPathByTypeVersion(string type)
        {
            var result = Path.Combine(GetPath(), $"Type{type}");
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }

        private string GetPath()
        {
            if(string.IsNullOrWhiteSpace(_options.SubPath)) return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "storage");
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _options.SubPath, "storage");
        }
    }
}
