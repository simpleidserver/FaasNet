using FaasNet.RaftConsensus.Core.Models;
using System;
using System.Text.Json;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class Vpn
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }

        public NodeState ToNodeState()
        {
            return new NodeState
            {
                EntityType = StandardEntityTypes.Vpn,
                EntityId = Guid.NewGuid().ToString(),
                EntityVersion = 0,
                Value = JsonSerializer.Serialize(this)
            };
        }

        public static Vpn Create(string name, string description)
        {
            return new Vpn
            {
                Name = name,
                Description = description,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
        }
    }
}
