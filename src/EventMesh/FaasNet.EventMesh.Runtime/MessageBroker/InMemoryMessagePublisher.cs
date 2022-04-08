using CloudNative.CloudEvents;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.MessageBroker
{
    public class InMemoryMessagePublisher : IMessagePublisher
    {
        private readonly ConcurrentBag<EventMeshCloudEvent> _events;

        public InMemoryMessagePublisher(ConcurrentBag<EventMeshCloudEvent> events)
        {
            _events = events;
        }

        public string BrokerName => Constants.InMemoryBrokername;

        public Task Publish(CloudEvent cloudEvent, string topicName, Models.Client client)
        {
            _events.Add(new EventMeshCloudEvent { CloudEvt = cloudEvent, Topic = topicName, CreateDateTime = DateTime.UtcNow });
            return Task.CompletedTask;
        }
    }
}
