using FaasNet.Peer;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FaasNet.Partition
{
    public class PartitionedNodeOptions
    {
        public PartitionedNodeOptions()
        {
            StartPeerPort = 30000;
            MaxConcurrentThreads = 3;
        }

        /// <summary>
        /// Peer start to listen on this port.
        /// </summary>
        public int StartPeerPort { get; set; }
        /// <summary>
        /// Maximum concurrent threads.
        /// </summary>
        public int MaxConcurrentThreads { get; set; }
        /// <summary>
        /// Configure dependencies of the Peers.
        /// </summary>
        public Action<IServiceCollection> CallbackPeerDependencies { get; set; } = null;
        /// <summary>
        /// Update Peer configurations.
        /// </summary>
        public Action<PeerHostFactory> CallbackPeerConfiguration { get; set; } = null;
    }
}
