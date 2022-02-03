using CloudNative.CloudEvents;
using System;

namespace EventMesh.Runtime.Events
{
    public class CloudEventArgs : EventArgs
    {
        public CloudEventArgs(string topic, CloudEvent evt)
        {
            Topic = topic;
            Evt = evt;
        }

        public string Topic { get; set; }
        public CloudEvent Evt { get; set; }
    }
}
