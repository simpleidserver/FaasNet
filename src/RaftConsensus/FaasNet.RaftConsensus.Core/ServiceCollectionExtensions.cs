using FaasNet.Common;
using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using System;
using System.Collections.Concurrent;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static ServerBuilder AddConsensusPeer(this IServiceCollection services, Action<ConsensusPeerOptions> callback = null)
        {
            if (callback != null) services.Configure(callback);
            else services.Configure<ConsensusPeerOptions>((o) => { });
            var peerStore = new InMemoryPeerStore(new ConcurrentBag<PeerInfo>());
            var nodeStateStore = new InMemoryNodeStateStore(new ConcurrentBag<NodeState>());
            services.AddLogging();
            services.AddTransient<INodeHost, StandaloneNodeHost>();
            services.AddTransient<IPeerHostFactory, PeerHostFactory>();
            services.AddTransient<IClusterStore, ClusterStore>();
            services.AddScoped<IPeerHost, StandalonePeerHost>();
            services.AddScoped<ILogStore, InMemoryLogStore>();
            services.AddSingleton<IPeerStore>(peerStore);
            services.AddSingleton<INodeStateStore>(nodeStateStore);
            return new ServerBuilder(services);
        }
    }
}
