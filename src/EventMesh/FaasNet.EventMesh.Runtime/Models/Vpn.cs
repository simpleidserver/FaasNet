using FaasNet.Domain.Exceptions;
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
            BridgeServers = new List<BridgeServer>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime UpdateDateTime { get; set; }
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

        public BridgeServer GetBridgeServer(string urn, int port, string vpn)
        {
            return BridgeServers.FirstOrDefault(bs => bs.Urn == urn && bs.Port == port && bs.Vpn == vpn);
        }
    }
}
