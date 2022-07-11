using System.Collections.Generic;

namespace FaasNet.CRDT.Client.Messages
{
    public class GSetDetla : IEntityDelta
    {
        public GSetDetla()
        {

        }

        public IEnumerable<string> Added { get; set; }
    }
}
