using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaasNet.DHT.Kademlia.Core
{
    public class DHTPeerInfo
    {
        public long Id { get; set; }

        public void Distance(long id, long id2)
        {
            // XOR between two nodes.
            var r = id ^ id2;

        }
    }
}
