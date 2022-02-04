using CloudNative.CloudEvents;

namespace EventMesh.Runtime.Models
{
    public class ClientSessionPendingCloudEvent
    {
        public CloudEvent Evt { get; set; }
        public string BrokerName { get; set; }
        public string Topic { get; set; }
    }
}
