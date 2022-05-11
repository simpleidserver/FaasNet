using FaasNet.EventMesh.Client.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public string Queue
        {
            get
            {
                return BuildQueueName(Id);
            }
        }

        public static string BuildQueueName(string clientId)
        {
            return $"queue-{clientId}";
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
