using FaasNet.Discovery.Etcd;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Peer
{
    public static class PeerHostFactoryExtensions
    {
        public static PeerHostFactory UseDiscoveryEtcd(this PeerHostFactory peerHostFactory, Action<EtcdOptions> callback = null)
        {
            peerHostFactory.Services.AddEtcdDiscovery(callback);
            return peerHostFactory;
        }
    }
}
