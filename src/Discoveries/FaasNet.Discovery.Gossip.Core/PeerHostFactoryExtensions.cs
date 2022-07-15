using FaasNet.Discovery.Gossip.Core;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Peer
{
    public static class PeerHostFactoryExtensions
    {
        public static PeerHostFactory UseGossipDiscovery(this PeerHostFactory factory, Action<GossipOptions> options = null)
        {
            if (options == null) factory.Services.Configure<GossipOptions>(o => { });
            else factory.Services.Configure(options);
            factory.Services.AddTransient<IProtocolHandler, GossipProtocolHandler>();
            factory.Services.AddTransient<ITimer, GossipTimer>();
            return factory;
        }
    }
}
