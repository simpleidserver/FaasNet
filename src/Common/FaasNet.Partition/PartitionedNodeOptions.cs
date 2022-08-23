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
            PeerConfiguration = null;
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
        /// Can be used to configure peer.
        /// </summary>
        public Action<IServiceCollection> PeerConfiguration { get; set; }
    }
}
