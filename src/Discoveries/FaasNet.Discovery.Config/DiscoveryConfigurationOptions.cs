using FaasNet.Peer.Clusters;
using FaasNet.Plugin;
using System.Collections.Generic;

namespace FaasNet.Discovery.Config
{
    public class DiscoveryConfigurationOptions
    {
        public DiscoveryConfigurationOptions()
        {
            ClusterNodes = new List<ClusterPeer>
            {
                new ClusterPeer("localhost", 4000)
            };
        }

        /// <summary>
        /// Cluster nodes.
        /// </summary>
        [PluginEntryOptionProperty("clusterNodes", "Cluster nodes")]
        public ICollection<ClusterPeer> ClusterNodes { get; set; }
    }
}
