﻿using RocksDbSharp;
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
        }

        /// <summary>
        /// Configure RockDB.
        /// </summary>
        public Action<DbOptions> OptionsCallback { get; set; }
    }
}
