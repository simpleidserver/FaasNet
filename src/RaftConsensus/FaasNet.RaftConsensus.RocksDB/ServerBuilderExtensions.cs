using FaasNet.RaftConsensus.Core.Stores;
using FaasNet.RaftConsensus.RocksDB;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Peer
{
    public static class ServerBuilderExtensions
    {
        public static PeerHostFactory UseRocksDB(this PeerHostFactory hostFactory, Action<RaftConsensusRocksDBOptions> callback = null)
        {
            if (callback == null) hostFactory.Services.Configure<RaftConsensusRocksDBOptions>(o => { });
            else hostFactory.Services.Configure(callback);
            hostFactory.Services.AddScoped<ILogStore, RocksDBLogStore>();
            hostFactory.Services.AddScoped<ISnapshotStore, RocksDBSnapshotStore>();
            return hostFactory;
        }
    }
}
