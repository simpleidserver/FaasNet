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
            lock(EventMeshDBContext.Lock)
            {
                _dbContext.BridgeServers.Add(bridgeServer);
                _dbContext.SaveChanges();
            }
        }

        public int Count()
        {
            lock (EventMeshDBContext.Lock)
            {
                return _dbContext.BridgeServers.Count();
            }
        }

        public BridgeServer Get(string urn)
        {
            lock (EventMeshDBContext.Lock)
            {
                return _dbContext.BridgeServers.FirstOrDefault(b => b.Urn == urn);
            }
        }

        public IEnumerable<BridgeServer> GetAll()
        {
            lock (EventMeshDBContext.Lock)
            {
                return _dbContext.BridgeServers.ToList();
            }
        }
    }
}
