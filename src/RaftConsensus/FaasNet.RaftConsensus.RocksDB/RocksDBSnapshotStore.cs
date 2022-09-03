using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FaasNet.RaftConsensus.RocksDB
{
    internal class RocksDBSnapshotStore : ISnapshotStore
    {
        private readonly RaftConsensusRocksDBOptions _rockDbOptions;
        private readonly RaftConsensusPeerOptions _raftOptions;
        private readonly RocksDBConnectionPool _connectionPool;

        public RocksDBSnapshotStore(IOptions<RaftConsensusRocksDBOptions> rockDbOptions, IOptions<RaftConsensusPeerOptions> raftOptions)
        {
            _rockDbOptions = rockDbOptions.Value;
            _raftOptions = raftOptions.Value;
            _connectionPool = new RocksDBConnectionPool();
        }

        public IEnumerable<Snapshot> GetAll()
        {
            var versionDb = _connectionPool.GetConnection(BuildOptions(), GetDirectoryPath());
            using (var iterator = versionDb.NewIterator())
            {
                while (iterator.Valid())
                {
                    var payload = iterator.Value();
                    if (payload != null) yield return Snapshot.Deserialize(payload);
                    iterator.Next();
                }
            }
        }

        public Snapshot Get(string stateMachineId)
        {
            Snapshot result = null;
            var versionDb = _connectionPool.GetConnection(BuildOptions(), GetDirectoryPath());
            var payload = versionDb.Get(Encoding.UTF8.GetBytes(stateMachineId));
            if (payload != null) result = Snapshot.Deserialize(payload);
            return result;
        }

        public void Update(Snapshot snapshot)
        {
            var versionDb = _connectionPool.GetConnection(BuildOptions(), GetDirectoryPath());
            versionDb.Put(Encoding.UTF8.GetBytes(snapshot.StateMachineId), snapshot.Serialize());
        }

        private RocksDbSharp.DbOptions BuildOptions()
        {
            var result = new RocksDbSharp.DbOptions();
            _rockDbOptions.OptionsCallback(result);
            return result;
        }

        private string GetDirectoryPath()
        {
            if (!Directory.Exists(_raftOptions.ConfigurationDirectoryPath)) Directory.CreateDirectory(_raftOptions.ConfigurationDirectoryPath);
            var result = Path.Combine(_raftOptions.ConfigurationDirectoryPath, "snapshots");
            if (!Directory.Exists(result)) Directory.CreateDirectory(result);
            return result;
        }
    }
}
