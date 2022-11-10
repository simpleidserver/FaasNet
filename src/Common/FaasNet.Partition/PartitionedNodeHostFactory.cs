using FaasNet.Peer;
using FaasNet.Peer.Client;
using FaasNet.Peer.Clusters;
using FaasNet.Peer.Transports;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace FaasNet.Partition
{
    public class PartitionedNodeHostFactory
    {
        private readonly IServiceCollection _serviceCollection;

        private PartitionedNodeHostFactory(Action<PeerOptions> options, Action<PartitionedNodeOptions> nodeOptions, ConcurrentBag<ClusterPeer> clusterPeers, Action<IServiceCollection> callbackService = null)
        {
            _serviceCollection = new ServiceCollection();
            if (options == null) _serviceCollection.Configure<PeerOptions>(o => { });
            else _serviceCollection.Configure(options);
            if (nodeOptions == null) _serviceCollection.Configure<PartitionedNodeOptions>(o => { });
            else _serviceCollection.Configure(nodeOptions);
            _serviceCollection.AddScoped<IPeerHost, PartitionedNodeHost>();
            if (clusterPeers != null) _serviceCollection.AddScoped<IClusterStore>(s => new InMemoryClusterStore(clusterPeers));
            else _serviceCollection.AddScoped<IClusterStore, InMemoryClusterStore>();
            _serviceCollection.AddTransient<IProtocolHandlerFactory, ProtocolHandlerFactory>();
            _serviceCollection.AddScoped<IPartitionCluster, DirectPartitionCluster>();
            _serviceCollection.AddSingleton<IPartitionPeerStore>(new InMemoryPartitionPeerStore(new ConcurrentBag<DirectPartitionPeer>()));
            _serviceCollection.AddTransient<IPeerClientFactory, PeerClientFactory>();
            _serviceCollection.AddLogging();
            if (callbackService != null) callbackService(_serviceCollection);
        }

        public static PartitionedNodeHostFactory New(Action<PeerOptions> options = null, Action<PartitionedNodeOptions> nodeOptions = null, ConcurrentBag<ClusterPeer> clusterNodes = null, Action<IServiceCollection> callbackService = null)
        {
            return new PartitionedNodeHostFactory(options, nodeOptions, clusterNodes, callbackService);
        }

        public IServiceCollection Services => _serviceCollection;

        public PartitionedNodeHostFactory UseServerTCPTransport()
        {
            RemoveServerTransport();
            _serviceCollection.AddScoped<IServerTransport, ServerTCPTransport>();
            return this;
        }

        public PartitionedNodeHostFactory UseUDPTransport()
        {
            UseServerUDPTransport();
            UseClientUDPTransport();
            return this;
        }

        public PartitionedNodeHostFactory UseTCPTransport()
        {
            UseServerTCPTransport();
            UseClientTCPTransport();
            return this;
        }

        public PartitionedNodeHostFactory UseServerUDPTransport()
        {
            RemoveServerTransport();
            _serviceCollection.AddScoped<IServerTransport, ServerUDPTransport>();
            return this;
        }

        public PartitionedNodeHostFactory UseClientTCPTransport()
        {
            RemoveClientTransport();
            _serviceCollection.AddTransient<IClientTransportFactory, TCPClientTransportFactory>();
            return this;
        }

        public PartitionedNodeHostFactory UseClientUDPTransport()
        {
            RemoveClientTransport();
            _serviceCollection.AddTransient<IClientTransportFactory, UDPClientTransportFactory>();
            return this;
        }

        /// <summary>
        /// One partition per cluster.
        /// </summary>
        public PartitionedNodeHostFactory UseDirectPartitionKey(params DirectPartitionPeer[]  partitionPeers)
        {
            RemovePartitionCluster();
            RemovePartitionPeerStore();
            _serviceCollection.AddScoped<IPartitionCluster, DirectPartitionCluster>();
            _serviceCollection.AddSingleton<IPartitionPeerStore>(new InMemoryPartitionPeerStore(new ConcurrentBag<DirectPartitionPeer>(partitionPeers.ToList())));
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

        private void RemovePartitionCluster() => RemoveDependency<IPartitionCluster>();

        private void RemoveServerTransport() => RemoveDependency<IServerTransport>();

        private void RemoveClientTransport() => RemoveDependency<IClientTransportFactory>();

        private void RemovePartitionPeerStore() => RemoveDependency<IPartitionPeerStore>();

        private void RemoveDependency<T>()
        {
            var registeredType = _serviceCollection.FirstOrDefault(s => s.ServiceType == typeof(T));
            if (registeredType != null) _serviceCollection.Remove(registeredType);
        }
    }
}
