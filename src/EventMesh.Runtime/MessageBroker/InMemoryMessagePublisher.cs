using CloudNative.CloudEvents;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventMesh.Runtime.MessageBroker
{
    public class InMemoryMessagePublisher : IMessagePublisher
    {
        private readonly ICollection<InMemoryTopic> _topics;

        public InMemoryMessagePublisher(ICollection<InMemoryTopic> topics)
        {
            _topics = topics;
        }

        public Task Publish(CloudEvent cloudEvent, string topicName)
        {
            var topic = _topics.First(t => t.TopicName == topicName);
            topic.PublishMessage(cloudEvent);
            return Task.CompletedTask;
        }
    }
}
