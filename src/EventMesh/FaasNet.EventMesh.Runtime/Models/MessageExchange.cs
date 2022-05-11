using FaasNet.RaftConsensus.Core.Models;
using System.Text.Json;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class MessageExchange
    {
        public string ClientId { get; set; }
        public string Topic { get; set; }

        public static string BuildId(string clientId, string topic)
        {
            return $"{clientId}_{topic}";
        }

        public NodeState ToNodeState()
        {
            return new NodeState
            {
                EntityType = StandardEntityTypes.MessageExchange,
                EntityId = BuildId(ClientId, Topic),
                EntityVersion = 0,
                Value = JsonSerializer.Serialize(this)
            };
        }
    }
}
