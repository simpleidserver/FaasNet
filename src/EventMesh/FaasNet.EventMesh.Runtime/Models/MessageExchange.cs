using FaasNet.RaftConsensus.Core.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class MessageExchange
    {
        public string TopicFilter { get; set; }
        public IEnumerable<string> ClientIds { get; set; }

        public bool IsMatch(string filter)
        {
            var regex = new Regex(TopicFilter);
            return regex.IsMatch(filter);
        }

        public NodeState ToNodeState()
        {
            return new NodeState
            {
                EntityType = StandardEntityTypes.MessageExchange,
                EntityId = TopicFilter,
                EntityVersion = 0,
                Value = JsonSerializer.Serialize(this)
            };
        }
    }
}
