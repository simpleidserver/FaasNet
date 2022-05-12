using FaasNet.RaftConsensus.Core;
using FaasNet.RaftConsensus.Core.Models;
using FaasNet.RaftConsensus.Core.Stores;
using System;
using System.Collections.Concurrent;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection RegisterConsensusPeer(this IServiceCollection services, Action<ConsensusPeerOptions> callback = null)
        {
            if (callback != null) services.Configure(callback);
            else services.Configure<ConsensusPeerOptions>((o) => { });
            var peerStore = new InMemoryPeerInfoStore(new ConcurrentBag<PeerInfo>());
            services.AddLogging();
            services.AddTransient<INodeHost, StandaloneNodeHost>();
            services.AddTransient<IPeerHostFactory, PeerHostFactory>();
            services.AddTransient<IClusterStore, InMemoryClusterStore>();
            services.AddTransient<IPeerStore, InMemoryPeerStore>();
            services.AddScoped<IPeerHost, StandalonePeerHost>();
            services.AddScoped<ILogStore, InMemoryLogStore>();
            services.AddSingleton<INodeStateStore, InMemoryNodeStateStore>();
            services.AddSingleton<IPeerInfoStore>(peerStore);
            return services;
        }
    }
}
