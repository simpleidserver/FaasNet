using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FaasNet.Common
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder SetPeers(this ServerBuilder serverBuilder, ConcurrentBag<PeerInfo> peerInfos)
        {
            var peerStore = new InMemoryPeerInfoStore(peerInfos);
            serverBuilder.Services.AddSingleton<IPeerInfoStore>(peerStore);
            return serverBuilder;
        }

        public static ServerBuilder SetNodeStates(this ServerBuilder serverBuilder, ConcurrentBag<NodeState> nodeStates)
        {
            var nodeStateStore = new InMemoryNodeStateStore(nodeStates);
            serverBuilder.Services.AddSingleton<INodeStateStore>(nodeStateStore);
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
