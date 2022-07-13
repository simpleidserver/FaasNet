﻿using FaasNet.Peer.Clusters;
using FaasNet.Peer.Transports;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace FaasNet.Peer
{
    public class PeerHostFactory
    {
        private readonly IServiceCollection _serviceCollection;

        private PeerHostFactory(Action<PeerOptions> options, ConcurrentBag<ClusterPeer> clusterPeers)
        {
            _serviceCollection = new ServiceCollection();
            if (options == null) _serviceCollection.Configure<PeerOptions>(o => { });
            else _serviceCollection.Configure(options);
            _serviceCollection.AddTransient<IPeerHost, PeerHost>();
            _serviceCollection.AddTransient<IProtocolHandlerFactory, ProtocolHandlerFactory>();
            if (clusterPeers != null) _serviceCollection.AddSingleton<IClusterStore>(new InMemoryClusterStore(clusterPeers));
            else _serviceCollection.AddSingleton<IClusterStore, InMemoryClusterStore>();
            _serviceCollection.AddLogging();
        }

        public IServiceCollection Services => _serviceCollection;

        public static PeerHostFactory New(Action<PeerOptions> options = null, ConcurrentBag<ClusterPeer> clusterNodes = null)
        {
            return new PeerHostFactory(options, clusterNodes);
        }

        public PeerHostFactory UseTCPTransport()
        {
            RemoveTransport();
            _serviceCollection.AddTransient<ITransport, TCPTransport>();
            return this;
        }

        public PeerHostFactory UseUDPTransport()
        {
            RemoveTransport();
            _serviceCollection.AddTransient<ITransport, UDPTransport>();
            return this;
        }

        public IPeerHost Build()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IPeerHost>();
        }

        private void RemoveTransport()
        {
            var registeredType = _serviceCollection.FirstOrDefault(s => s.ServiceType == typeof(ITransport));
            if (registeredType != null) _serviceCollection.Remove(registeredType);
        }
    }
}
