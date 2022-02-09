using CloudNative.CloudEvents;
using EventMesh.Runtime.Models;
using System;

namespace EventMesh.Runtime.Events
{
    public class CloudEventArgs : EventArgs
    {
        public CloudEventArgs(string topic, string brokerName, CloudEvent evt, string clientId, ClientSession clientSession)
        {
            Topic = topic;
            BrokerName = brokerName;
            Evt = evt;
            ClientId = clientId;
            ClientSession = clientSession;
        }

        public string Topic { get; set; }
        public string BrokerName { get; set; }
        public CloudEvent Evt { get; set; }
        public string ClientId { get; set; }
        public ClientSession ClientSession { get; set; }
    }
}
