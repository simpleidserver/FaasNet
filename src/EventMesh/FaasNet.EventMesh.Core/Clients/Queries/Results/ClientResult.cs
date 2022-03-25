using FaasNet.EventMesh.Runtime.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Core.Clients.Queries.Results
{
    public class ClientResult
    {
        public ClientResult()
        {
            Sessions = new List<ClientSessionResult>();
        }

        public string ClientId { get; set; }
        public ICollection<int> Purposes { get; set; }
        public IEnumerable<ClientSessionResult> Sessions { get; set; }
        public DateTime CreateDateTime { get; set; }

        public static ClientResult Build(Client client)
        {
            return new ClientResult
            {
                ClientId = client.ClientId,
                Purposes = client.Purposes,
                Sessions = client.Sessions.Select(s => ClientSessionResult.Build(s)),
                CreateDateTime = client.CreateDateTime
            };
        }
    }
}
