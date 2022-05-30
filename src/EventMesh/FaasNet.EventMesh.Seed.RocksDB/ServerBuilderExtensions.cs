using FaasNet.EventMesh.Seed.RocksDB;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseSeedRocksDB(this ServerBuilder serverBuilder, Action<EventMeshSeedRocksDBOptions> callback = null)
        {
            serverBuilder.Services.UseSeedRocksDB(callback);
            return serverBuilder;
        }
    }
}
