using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;
using System.Collections.Generic;
using System.Linq;

namespace EventMesh.Runtime.EF.Stores
{
    public class EFBridgeServerStore : IBridgeServerStore
    {
        private readonly EventMeshDBContext _dbContext;

        public EFBridgeServerStore(EventMeshDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(BridgeServer bridgeServer)
        {
            _dbContext.BridgeServers.Add(bridgeServer);
            _dbContext.SaveChanges();
        }

        public BridgeServer Get(string urn)
        {
            return _dbContext.BridgeServers.FirstOrDefault(b => b.Urn == urn);
        }

        public IEnumerable<BridgeServer> GetAll()
        {
            return _dbContext.BridgeServers.ToList();
        }
    }
}
