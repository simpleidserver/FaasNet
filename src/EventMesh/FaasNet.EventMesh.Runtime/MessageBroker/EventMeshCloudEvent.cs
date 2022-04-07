using CloudNative.CloudEvents;

namespace FaasNet.EventMesh.Runtime.MessageBroker
{
    public class EventMeshCloudEvent
    {
        public CloudEvent CloudEvt { get; set; }
        public string Topic { get; set; }
    }
}
