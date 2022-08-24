using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;

namespace FaasNet.RaftConsensus.Core
{
    public class RaftConsensusPartitionPeerFactory : IPartitionPeerFactory
    {
        private readonly IClusterStore _clusterStore;

        public RaftConsensusPartitionPeerFactory(IClusterStore clusterStore)
        {
            _clusterStore = clusterStore;
        }

        public IPeerHost Build(int port, string partitionKey, Action<IServiceCollection> callbackService = null, Action<PeerHostFactory> callbackHostFactory = null)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var hostFactory = PeerHostFactory.NewUnstructured(o => {
                o.Port = port;
                o.PartitionKey = partitionKey;
            }, callbackService: callbackService)
                .UseServerUDPTransport()
                .UseClientUDPTransport()
                .UseClusterStore(_clusterStore)
                .AddRaftConsensus(o =>
                {
                    o.ConfigurationDirectoryPath = Path.Combine(path, port.ToString());
                });
            if (callbackHostFactory != null) callbackHostFactory(hostFactory);
            return hostFactory.Build();
        }
    }
}
