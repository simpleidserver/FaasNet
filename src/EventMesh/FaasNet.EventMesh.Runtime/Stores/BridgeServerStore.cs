using FaasNet.EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Runtime.Stores
{
    public class BridgeServerStore : IBridgeServerStore
    {
        private readonly ICollection<BridgeServer> _bridgeServers;

        public BridgeServerStore()
        {
            _bridgeServers = new List<BridgeServer>();
        }

        public void Add(BridgeServer bridgeServer)
        {
            _bridgeServers.Add(bridgeServer);
        }

        public int Count()
        {
            return _bridgeServers.Count();
        }

        public BridgeServer Get(string urn)
        {
            return _bridgeServers.FirstOrDefault(b => b.Urn == urn);
        }

        public IEnumerable<BridgeServer> GetAll()
        {
            return _bridgeServers;
        }
    }
}
