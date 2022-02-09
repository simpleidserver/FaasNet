using EventMesh.Runtime.Models;
using System.Collections.Generic;
using System.Linq;

namespace EventMesh.Runtime.Stores
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
