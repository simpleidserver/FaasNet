using CloudNative.CloudEvents;
using FaasNet.EventMesh.Runtime.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaasNet.EventMesh.Runtime.MessageBroker
{
    public class InMemoryTopic
    {
        public InMemoryTopic()
        {
            CloudEvts = new List<CloudEvent>();
            Subscriptions = new List<InMemoryTopicSubscription>();
        }

        public string TopicName { get; set; }
        public ICollection<CloudEvent> CloudEvts { get; set; }
        public ICollection<InMemoryTopicSubscription> Subscriptions { get; set; }
        public event EventHandler<CloudEventArgs> CloudEventReceived;

        public void PublishMessage(CloudEvent cloudEvt)
        {
            CloudEvts.Add(cloudEvt);
            foreach(var subscription in Subscriptions.Where(s => s.Session.State == Models.ClientSessionState.ACTIVE))
            {
                var pendingCloudEvents = CloudEvts.Skip(subscription.Offset);
                foreach(var pendingCloudEvt in pendingCloudEvents)
                {
                    CloudEventReceived(this, new CloudEventArgs(TopicName, Constants.InMemoryBrokername, pendingCloudEvt, subscription.ClientId, subscription.Session));
                }
            }
        }
    }
}
