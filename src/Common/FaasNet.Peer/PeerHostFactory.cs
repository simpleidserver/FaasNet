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

        private PeerHostFactory(Action<PeerOptions> options, Action<IServiceCollection> callbackService = null)
        {
            _serviceCollection = new ServiceCollection();
            if (options == null) _serviceCollection.Configure<PeerOptions>(o => { });
            else _serviceCollection.Configure(options);
            _serviceCollection.AddScoped<IPeerHost, StructuredPeerHost>();
            _serviceCollection.AddTransient<IProtocolHandlerFactory, ProtocolHandlerFactory>();
            _serviceCollection.AddLogging();
            if (callbackService != null) callbackService(_serviceCollection);
        }

        private PeerHostFactory(Action<PeerOptions> options, ConcurrentBag<ClusterPeer> clusterPeers, Action<IServiceCollection> callbackService = null)
        {
            _serviceCollection = new ServiceCollection();
            if (options == null) _serviceCollection.Configure<PeerOptions>(o => { });
            else _serviceCollection.Configure(options);
            _serviceCollection.AddScoped<IPeerHost, UnstructuredPeerHost>();
            _serviceCollection.AddTransient<IProtocolHandlerFactory, ProtocolHandlerFactory>();
            if (clusterPeers != null) _serviceCollection.AddScoped<IClusterStore>(s => new InMemoryClusterStore(clusterPeers));
            else _serviceCollection.AddScoped<IClusterStore, InMemoryClusterStore>();
            _serviceCollection.AddLogging();
            if (callbackService != null) callbackService(_serviceCollection);
        }

        public IServiceCollection Services => _serviceCollection;

        public static PeerHostFactory NewUnstructured(Action<PeerOptions> options = null, ConcurrentBag<ClusterPeer> clusterNodes = null, Action<IServiceCollection> callbackService = null)
        {
            return new PeerHostFactory(options, clusterNodes, callbackService);
        }

        public static PeerHostFactory NewStructured(Action<PeerOptions> options = null, Action<IServiceCollection> callbackService = null)
        {
            return new PeerHostFactory(options, callbackService);
        }

        public PeerHostFactory UseTCPTransport()
        {
            RemoveTransport();
            _serviceCollection.AddScoped<ITransport, TCPTransport>();
            return this;
        }

        public PeerHostFactory UseUDPTransport()
        {
            RemoveTransport();
            _serviceCollection.AddScoped<ITransport, UDPTransport>();
            return this;
        }

        public IPeerHost Build()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IPeerHost>();
        }

        public (IPeerHost, IServiceProvider) BuildWithDI()
        {
            var serviceProvider = _serviceCollection.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();
            return (scope.ServiceProvider.GetRequiredService<IPeerHost>(), scope.ServiceProvider);
        }

        private void RemoveTransport()
        {
            var registeredType = _serviceCollection.FirstOrDefault(s => s.ServiceType == typeof(ITransport));
            if (registeredType != null) _serviceCollection.Remove(registeredType);
        }
    }
}
