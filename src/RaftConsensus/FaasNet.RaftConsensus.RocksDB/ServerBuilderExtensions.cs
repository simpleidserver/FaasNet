using FaasNet.RaftConsensus.Core.Stores;
using FaasNet.RaftConsensus.RocksDB;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseRocksDB(this ServerBuilder serverBuilder, Action<RaftConsensusRocksDBOptions> callback = null)
        {
            if (callback == null) serverBuilder.Services.Configure<RaftConsensusRocksDBOptions>(o => { });
            else serverBuilder.Services.Configure(callback);
            serverBuilder.Services.AddSingleton<INodeStateStore, RockDBNodeStateStore>();
            return serverBuilder;
        }
    }
}
