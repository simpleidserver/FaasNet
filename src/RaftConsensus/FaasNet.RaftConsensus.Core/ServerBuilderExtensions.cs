using FaasNet.Common;
using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace FaasNet.RaftConsensus.Core
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder SetPeers(this ServerBuilder serverBuilder, ConcurrentBag<PeerInfo> peerInfos)
        {
            var peerStore = new InMemoryPeerStore(peerInfos);
            serverBuilder.Services.AddSingleton<IPeerStore>(peerStore);
            return serverBuilder;
        }

        public static ServerBuilder SetClusterNodes(this ServerBuilder serverBuilder, ConcurrentBag<ClusterNode> nodes)
        {
            var clusterStore = new InMemoryClusterStore(nodes);
            serverBuilder.Services.AddSingleton<IClusterStore>(clusterStore);
            return serverBuilder;
        }

        public static ServerBuilder UseLogFileStore(this ServerBuilder serverBuilder, Action<FileLogStoreOptions> callback)
        {
            if (callback != null) serverBuilder.Services.Configure(callback);
            else serverBuilder.Services.Configure<ConsensusPeerOptions>((o) => { });
            serverBuilder.Services.AddScoped<ILogStore, FileLogStore>();
            return serverBuilder;
        }
    }
}
