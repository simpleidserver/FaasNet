using CloudNative.CloudEvents;
using System;

namespace FaasNet.EventMesh.Runtime.MessageBroker
{
    public class EventMeshCloudEvent
    {
        public CloudEvent CloudEvt { get; set; }
        public string Topic { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
