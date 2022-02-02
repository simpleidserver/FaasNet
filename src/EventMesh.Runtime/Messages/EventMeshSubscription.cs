using System.Collections.Generic;

namespace EventMesh.Runtime.Messages
{
    public class EventMeshSubscription
    {
        public IEnumerable<EventMeshSubscriptionItem> Topics { get; set; }
    }
}
