using FaasNet.EventMesh.Plugin;
using FaasNet.RaftConsensus.Core.Models;
using System.Collections.Generic;

namespace FaasNet.RaftConsensus.Discovery.Config
{
    public class DiscoveryConfigurationOptions
    {
        public DiscoveryConfigurationOptions()
        {
            ClusterNodes = new List<ClusterNode>
            {
                new ClusterNode { Url = "localhost", Port = 4000 }
            };
        }

        /// <summary>
        /// Cluster nodes.
        /// </summary>
        [PluginEntryOptionProperty("clusterNodes", "Cluster nodes")]
        public ICollection<ClusterNode> ClusterNodes { get; set; }
    }
}
