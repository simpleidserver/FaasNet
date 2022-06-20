using FaasNet.RaftConsensus.Core.Models;
using System;
using System.Text.Json;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class BridgeServer
    {
        public string SourceVpn { get; set; }
        public string TargetUrn { get; set; }
        public int TargetPort { get; set; }
        public string TargetVpn { get; set; }
        public string TargetClientId { get; set; }

        public NodeState ToNodeState()
        {
            return new NodeState
            {
                EntityType = StandardEntityTypes.BridgeServer,
                EntityId = Guid.NewGuid().ToString(),
                EntityVersion = 0,
                Value = JsonSerializer.Serialize(this)
            };
        }

        public static BridgeServer Create(string sourceVpn, string targetUrn, int targetPort, string targetVpn)
        {
            return new BridgeServer
            {
                SourceVpn = sourceVpn,
                TargetUrn = targetUrn,
                TargetPort = targetPort,
                TargetVpn = targetVpn
            };
        }
    }
}
