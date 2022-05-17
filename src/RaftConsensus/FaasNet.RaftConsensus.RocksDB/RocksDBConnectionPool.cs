using RocksDbSharp;
using System.Collections.Generic;

namespace FaasNet.RaftConsensus.RocksDB
{
    public class RocksDBConnectionPool
    {
        private static object _lock = new object();
        private static Dictionary<string, RocksDb> _dic = new Dictionary<string, RocksDb>();

        public RocksDb GetConnection(DbOptions options, string path)
        {
            lock(_lock)
            {
                if (_dic.ContainsKey(path)) return _dic[path];
                var db = RocksDb.Open(options, path);
                _dic.Add(path, db);
                return db;
            }
        }
    }
}
