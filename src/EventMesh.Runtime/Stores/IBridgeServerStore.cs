using EventMesh.Runtime.Models;
using System.Collections.Generic;

namespace EventMesh.Runtime.Stores
{
    public interface IBridgeServerStore
    {
        BridgeServer Get(string urn);
        void Add(BridgeServer bridgeServer);
        IEnumerable<BridgeServer> GetAll();
        int Count();
    }
}
