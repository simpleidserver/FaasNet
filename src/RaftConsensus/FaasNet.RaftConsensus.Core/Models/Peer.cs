using FaasNet.RaftConsensus.Client;
using System;
using System.Text.Json;

namespace FaasNet.RaftConsensus.Core.Models
{
    public class Peer
    {
        public string TermId { get; set; }

        public NodeState ToNodeState()
        {
            return new NodeState
            {
                EntityType = StandardEntityTypes.Peer,
                EntityId = Guid.NewGuid().ToString(),
                EntityVersion = 0,
                Value = JsonSerializer.Serialize(this)
            };
        }
    }
}
