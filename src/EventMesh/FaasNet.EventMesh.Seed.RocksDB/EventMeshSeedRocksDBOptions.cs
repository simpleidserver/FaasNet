using RocksDbSharp;
using System;

namespace FaasNet.EventMesh.Seed.RocksDB
{
    public class EventMeshSeedRocksDBOptions
    {
        public EventMeshSeedRocksDBOptions()
        {
            OptionsCallback = (opt) =>
            {
                opt.SetCreateIfMissing(true);
            };
            SubPath = string.Empty;
        }

        public Action<DbOptions> OptionsCallback { get; set; }
        public string SubPath { get; set; }
    }
}
