using FaasNet.Peer;
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

        private PartitionedNodeHostFactory(Action<PeerOptions> options,ConcurrentBag<ClusterPeer> clusterPeers, Action<IServiceCollection> callbackService = null)
        {
            _serviceCollection = new ServiceCollection();
            if (options == null) _serviceCollection.Configure<PeerOptions>(o => { });
            else _serviceCollection.Configure(options);
            _serviceCollection.AddScoped<IPeerHost, PartitionedNodeHost>();
            if (clusterPeers != null) _serviceCollection.AddScoped<IClusterStore>(s => new InMemoryClusterStore(clusterPeers));
            else _serviceCollection.AddScoped<IClusterStore, InMemoryClusterStore>();
            _serviceCollection.AddTransient<IProtocolHandlerFactory, ProtocolHandlerFactory>();
            _serviceCollection.AddLogging();
            if (callbackService != null) callbackService(_serviceCollection);
        }

        public static PartitionedNodeHostFactory New(Action<PeerOptions> options = null, ConcurrentBag<ClusterPeer> clusterNodes = null, Action<IServiceCollection> callbackService = null)
        {
            return new PartitionedNodeHostFactory(options, clusterNodes, callbackService);
        }

        public IServiceCollection Services => _serviceCollection;

        public PartitionedNodeHostFactory UseTCPTransport()
        {
            RemoveTransport();
            _serviceCollection.AddScoped<ITransport, TCPTransport>();
            return this;
        }

        public PartitionedNodeHostFactory UseUDPTransport()
        {
            RemoveTransport();
            _serviceCollection.AddScoped<ITransport, UDPTransport>();
            return this;
        }

        /// <summary>
        /// One partition per cluster.
        /// </summary>
        public PartitionedNodeHostFactory UseDirectPartitionKey(params DirectPartitionPeer[]  partitionPeers)
        {
            RemovePartitionPeerStore();
            _serviceCollection.AddScoped<IPartitionCluster, DirectPartitionCluster>();
            _serviceCollection.AddSingleton<IPartitionPeerStore>(new InMemoryPartitionPeerStore(partitionPeers));
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

        private void RemovePartitionPeerStore()
        {
            var registeredType = _serviceCollection.FirstOrDefault(s => s.ServiceType == typeof(IPartitionPeerStore));
            if (registeredType != null) _serviceCollection.Remove(registeredType);
        }
    }
}
