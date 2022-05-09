using FaasNet.RaftConsensus.Client;
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
                EntityId = string.Empty,
                EntityType = StandardEntityTypes.Cluster,
                EntityVersion = 0,
                Value = JsonSerializer.Serialize(this)
            };
        }
    }
}
