using FaasNet.EventMesh.Sink.RocksDB;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder UseSinkRocksDB(this ServerBuilder serverBuilder, Action<EventMeshSinkRocksDBOptions> callback = null)
        {
            serverBuilder.Services.UseSinkRocksDB(callback);
            return serverBuilder;
        }
    }
}
