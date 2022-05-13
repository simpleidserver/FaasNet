using FaasNet.EventMesh.Client.Messages;
using FaasNet.RaftConsensus.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class Client
    {
        public Client() { }

        private Client(string id, string clientId, string urn)
        {
            Id = id;
            ClientId = clientId;
            Urn = urn;
            Purposes = new List<int>();
        }

        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Urn { get; set; }
        public string Vpn { get; set; }
        public DateTime CreateDateTime { get; set; }
        public ICollection<int> Purposes { get; set; }

        public static string BuildId(string vpn, string clientId)
        {
            return $"{vpn}_{clientId}";
        }

        public NodeState ToNodeState()
        {
            return new NodeState
            {
                EntityType = StandardEntityTypes.Client,
                EntityId = BuildId(Vpn, ClientId),
                EntityVersion = 0,
                Value = JsonSerializer.Serialize(this)
            };
        }

        public static Client Create(string vpn, string clientId, string urn, List<UserAgentPurpose> purposes = null)
        {
            if (purposes == null)
            {
                purposes = new List<UserAgentPurpose>
                {
                    UserAgentPurpose.SUB
                };
            }

            return new Client(Guid.NewGuid().ToString(), clientId, urn)
            {
                Vpn = vpn,
                CreateDateTime = DateTime.UtcNow,
                Purposes = purposes.Select(p => p.Code).ToList()
            };
        }
    }
}
