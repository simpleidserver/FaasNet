﻿using FaasNet.RaftConsensus.Client;
using System;
using System.Text.Json;

namespace FaasNet.RaftConsensus.Core.Models
{
    public class ClusterNode
    {
        public string Url { get; set; }
        public int Port { get; set; }

        public NodeState ToNodeState()
        {
            return new NodeState
            {
                EntityType = StandardEntityTypes.Cluster,
                EntityId = Guid.NewGuid().ToString(),
                EntityVersion = 0,
                Value = JsonSerializer.Serialize(this)
            };
        }
    }
}
