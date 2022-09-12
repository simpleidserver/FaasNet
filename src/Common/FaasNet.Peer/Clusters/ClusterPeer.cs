﻿using FaasNet.Common;

namespace FaasNet.Peer.Clusters
{
    public class ClusterPeer
    {
        public ClusterPeer(string url, int port)
        {
            Url = url;
            Port = port;
        }

        public string Url { get; private set; }
        public int Port { get; private set; }
        public string PartitionKey { get; set; }
        public string Id
        {
            get
            {
                return PeerId.Build(Url, Port).Serialize();
            }
        }
    }
}
