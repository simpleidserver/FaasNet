using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
using RocksDbSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly RocksDBConnectionPool _connectionPool;

        public RockDBNodeStateStore(IOptions<RaftConsensusRocksDBOptions> options)
        {
            _options = options.Value;
            _connectionPool = new RocksDBConnectionPool();
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
            await _lock.WaitAsync(cancellationToken);
            var result = new List<NodeState>();
            var db = _connectionPool.GetConnection(BuildOptions(), GetFolderPathByTypeValue());
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

        public async Task<IEnumerable<NodeState>> GetAllLastEntityTypes(string entityType, CancellationToken cancellationToken)
        {
            await _lock.WaitAsync(cancellationToken);
            var result = new List<NodeState>();
            var db = _connectionPool.GetConnection(BuildOptions(), GetFolderPathByTypeVersion(entityType));
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

        public async Task<NodeState> GetSpecificEntityType(string entityType, int entityVersion, CancellationToken cancellationToken)
        {
            var entityTypes = await GetAllSpecificEntityTypes(new List<(string EntityType, int EntityVersion)> { (EntityType: entityType, EntityVersion: entityVersion) }, cancellationToken);
            return entityTypes.FirstOrDefault();
        }

        public async Task<IEnumerable<NodeState>> GetAllSpecificEntityTypes(List<(string EntityType, int EntityVersion)> parameter, CancellationToken cancellationToken)
        {
            await _lock.WaitAsync(cancellationToken);
            var result = new List<NodeState>();
            foreach (var grp in parameter.GroupBy(k => k.EntityType))
            {
                var db = _connectionPool.GetConnection(BuildOptions(), GetFolderPathByHistoryTypeVersionValue(grp.Key));
                foreach (var record in grp)
                {
                    var value = db.Get(record.EntityVersion.ToString());
                    if (string.IsNullOrWhiteSpace(value)) continue;
                    result.Add(JsonSerializer.Deserialize<NodeState>(value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
                }
            }

            _lock.Release();
            return result;
        }

        public async Task<NodeState> GetLastEntityId(string entityId, CancellationToken cancellationToken)
        {
            await _lock.WaitAsync(cancellationToken);
            var db = _connectionPool.GetConnection(BuildOptions(), GetFolderPathByLastIdValue());
            var value = db.Get(entityId);
            _lock.Release();
            return await Parse(value);
        }

        public async Task<NodeState> GetLastEntityType(string entityType, CancellationToken cancellationToken)
        {
            await _lock.WaitAsync(cancellationToken);
            var db = _connectionPool.GetConnection(BuildOptions(), GetFolderPathByTypeValue());
            var value = db.Get(entityType);
            _lock.Release();
            return await Parse(value);
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
            var db = _connectionPool.GetConnection(BuildOptions(), GetFolderPathByLastIdValue());
            db.Put(nodeState.EntityId, json);
            db = _connectionPool.GetConnection(BuildOptions(), GetFolderPathByLastVersionValue());
            db.Put(nodeState.EntityId, nodeState.EntityVersion.ToString());
        }

        private void StoreEntityType(NodeState nodeState, string json)
        {
            var db = _connectionPool.GetConnection(BuildOptions(), GetFolderPathByTypeValue());
            db.Put(nodeState.EntityType, json);
            db = _connectionPool.GetConnection(BuildOptions(), GetFolderPathByHistoryTypeVersionValue(nodeState.EntityType));
            db.Put(nodeState.EntityVersion.ToString(), json);
            db = _connectionPool.GetConnection(BuildOptions(), GetFolderPathByTypeVersion(nodeState.EntityType));
            db.Put(nodeState.EntityId, json);
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
            if(string.IsNullOrWhiteSpace(_options.SubPath)) return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "storage");
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _options.SubPath, "storage");
        }
    }
}
