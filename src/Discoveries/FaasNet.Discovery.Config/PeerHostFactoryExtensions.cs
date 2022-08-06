using FaasNet.Discovery.Config;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Peer
{
    public static class PeerHostFactoryExtensions
    {
        public static PeerHostFactory UseDiscoveryConfig(this PeerHostFactory peerHostFactory, Action<DiscoveryConfigurationOptions> callback = null)
        {
            peerHostFactory.Services.AddConfigDiscovery(callback);
            return peerHostFactory;
        }
    }
}
