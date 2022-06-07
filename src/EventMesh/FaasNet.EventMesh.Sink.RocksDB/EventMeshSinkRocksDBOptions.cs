using RocksDbSharp;
using System;

namespace FaasNet.EventMesh.Sink.RocksDB
{
    public class EventMeshSinkRocksDBOptions
    {
        public EventMeshSinkRocksDBOptions()
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
