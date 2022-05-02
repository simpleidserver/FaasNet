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
            var clusterStore = new InMemoryClusterStore(new ConcurrentBag<ClusterNode>());
            var peerStore = new InMemoryPeerStore(new ConcurrentBag<PeerInfo>());
            services.AddLogging();
            services.AddTransient<INodeHost, StandaloneNodeHost>();
            services.AddTransient<IPeerHostFactory, PeerHostFactory>();
            services.AddScoped<IPeerHost, StandalonePeerHost>();
            services.AddSingleton<IClusterStore>(clusterStore);
            services.AddSingleton<IPeerStore>(peerStore);
            return new ServerBuilder(services);
        }
    }
}
