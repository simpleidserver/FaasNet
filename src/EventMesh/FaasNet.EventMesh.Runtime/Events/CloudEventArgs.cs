using CloudNative.CloudEvents;
using FaasNet.EventMesh.Runtime.Models;
using System;

namespace FaasNet.EventMesh.Runtime.Events
{
    public class CloudEventArgs : EventArgs
    {
        public CloudEventArgs(string topicMessage, string topicFilter, string brokerName, CloudEvent evt, string clientId, ClientSession clientSession)
        {
            TopicMessage = topicMessage;
            TopicFilter = topicFilter;
            BrokerName = brokerName;
            Evt = evt;
            ClientId = clientId;
            ClientSession = clientSession;
        }

        public string TopicMessage { get; set; }
        public string TopicFilter { get; set; }
        public string BrokerName { get; set; }
        public CloudEvent Evt { get; set; }
        public string ClientId { get; set; }
        public ClientSession ClientSession { get; set; }
    }
}
