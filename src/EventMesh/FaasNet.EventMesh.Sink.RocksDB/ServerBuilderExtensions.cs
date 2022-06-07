using FaasNet.EventMesh.Sink.RocksDB;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseSeedRocksDB(this ServerBuilder serverBuilder, Action<EventMeshSinkRocksDBOptions> callback = null)
        {
            serverBuilder.Services.UseSeedRocksDB(callback);
            return serverBuilder;
        }
    }
}
