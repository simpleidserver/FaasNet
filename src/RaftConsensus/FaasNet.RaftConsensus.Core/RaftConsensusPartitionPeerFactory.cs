using FaasNet.Partition;
using FaasNet.Peer;
using FaasNet.Peer.Clusters;
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

        public IPeerHost Build(int port, string partitionKey)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return PeerHostFactory.NewUnstructured(o => {
                o.Port = port;
                o.PartitionKey = partitionKey;
            })
                .UseUDPTransport()
                .UseClusterStore(_clusterStore)
                .AddRaftConsensus(o =>
                {
                    o.ConfigurationDirectoryPath = Path.Combine(path, port.ToString());
                    o.LeaderCallback = () =>
                    {
                        string ss = "";
                    };
                })
                .Build();
        }
    }
}
