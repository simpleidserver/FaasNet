using FaasNet.CRDT.Client.Messages;
using System.Collections.Generic;

namespace FaasNet.CRDT.Core.SerializedEntities
{
    public class SerializedEntity
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public ICollection<ClockValue> ClockVector { get; set; }
    }
}
