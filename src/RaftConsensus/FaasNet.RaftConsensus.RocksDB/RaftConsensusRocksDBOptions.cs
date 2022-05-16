using RocksDbSharp;
using System;

namespace FaasNet.RaftConsensus.RocksDB
{
    public class RaftConsensusRocksDBOptions
    {
        public RaftConsensusRocksDBOptions()
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
