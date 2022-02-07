using EventMesh.Runtime.Events;
using EventMesh.Runtime.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.MessageBroker
{
    public class InMemoryMessageConsumer : IMessageConsumer
    {
        public event EventHandler<CloudEventArgs> CloudEventReceived;
        private readonly ICollection<InMemoryTopic> _topics;

        public InMemoryMessageConsumer(ICollection<InMemoryTopic> topics)
        {
            _topics = topics;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task Stop(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task Subscribe(string topicName, Client client, CancellationToken cancellationToken)
        {
            var topic = _topics.First(t => t.TopicName == topicName);
            var t = client.GetTopic(topicName, Constants.InMemoryBrokername);
            if(t == null)
            {
                t = client.AddTopic(topicName, Constants.InMemoryBrokername);
            }

            var subscription = new InMemoryTopicSubscription { Session = client.ActiveSession, Offset = t.Offset };
            topic.Subscriptions.Add(subscription);
            topic.CloudEventReceived += (e, o) => CloudEventReceived(e, o);
            return Task.CompletedTask;
        }

        public Task Unsubscribe(string topic, Client client, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
    }
}
