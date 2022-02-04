using CloudNative.CloudEvents;
using EventMesh.Runtime.Models;
using System;

namespace EventMesh.Runtime.Events
{
    public class CloudEventArgs : EventArgs
    {
        public CloudEventArgs(string topic, string brokerName, CloudEvent evt, ClientSession clientSession)
        {
            Topic = topic;
            BrokerName = brokerName;
            Evt = evt;
            ClientSession = clientSession;
        }

        public string Topic { get; set; }
        public string BrokerName { get; set; }
        public CloudEvent Evt { get; set; }
        public ClientSession ClientSession { get; set; }
    }
}
