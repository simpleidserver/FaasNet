using FaasNet.Domain.Exceptions;
using FaasNet.EventMesh.Runtime.Messages;
using FaasNet.EventMesh.Runtime.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Runtime.Models
{
    public class Vpn
    {
        public Vpn()
        {
            ApplicationDomains = new List<ApplicationDomain>();
            Clients = new List<Client>();
            BridgeServers = new List<BridgeServer>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public ICollection<ApplicationDomain> ApplicationDomains { get; set; }
        public ICollection<Client> Clients { get; set; }
        public ICollection<BridgeServer> BridgeServers { get; set; }

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

        public ApplicationDomain AddApplicationDomain(string name, string description, string rootTopic)
        {
            var record = new ApplicationDomain
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                RootTopic = rootTopic,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow
            };
            ApplicationDomains.Add(record);
            return record;
        }

        public void RemoveApplicationDomain(string id)
        {
            var applicationDomain = ApplicationDomains.FirstOrDefault(ad => ad.Id == id);
            if (applicationDomain == null)
            {
                throw new DomainException(ErrorCodes.UNKNOWN_APPLICATION_DOMAIN, string.Format(Global.UnknownApplicationDomain, id));
            }

            ApplicationDomains.Remove(applicationDomain);
        }

        public void AddBridge(string urn, int port, string vpn)
        {
            var bridgeServer = GetBridgeServer(urn, port, vpn);
            if (bridgeServer != null)
            {
                throw new DomainException(ErrorCodes.BRIDGE_ALREADY_EXISTS, Global.BridgeAlreadyExists);
            }

            BridgeServers.Add(new BridgeServer
            {
                Port = port,
                Urn = urn,
                Vpn = vpn
            });
        }

        public Client AddClient(string clientId, string urn, List<UserAgentPurpose> purposes = null)
        {
            if (purposes == null)
            {
                purposes = new List<UserAgentPurpose>
                {
                    UserAgentPurpose.SUB
                };
            }

            var client = Client.Create(clientId, urn);
            client.Purposes = purposes.Select(p => p.Code).ToList();
            Clients.Add(client);
            return client;
        }

        public Client GetClient(string clientId)
        {
            return Clients.FirstOrDefault(c => c.ClientId == clientId);
        }

        public Client GetByActiveSession(string clientId, string sessionId)
        {
            return Clients.FirstOrDefault(c => c.ClientId == clientId && c.Sessions.Any(s => s.Id == sessionId));
        }

        public BridgeServer GetBridgeServer(string urn, int port, string vpn)
        {
            return BridgeServers.FirstOrDefault(bs => bs.Urn == urn && bs.Port == port && bs.Vpn == vpn);
        }
    }
}
