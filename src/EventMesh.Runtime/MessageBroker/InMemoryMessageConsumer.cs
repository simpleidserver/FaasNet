using EventMesh.Runtime.Events;
using EventMesh.Runtime.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.MessageBroker
{
    public class InMemoryMessageConsumer : IMessageConsumer
    {
        public event EventHandler<CloudEventArgs> CloudEventReceived;
        private readonly ConcurrentBag<InMemoryTopic> _topics;

        public InMemoryMessageConsumer(ConcurrentBag<InMemoryTopic> topics)
        {
            _topics = topics;
        }

        public string BrokerName
        {
            get
            {
                return Constants.InMemoryBrokername;
            }
        }

        public Task Start(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task Stop(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task Subscribe(string topicName, Client client, string sessionId, CancellationToken cancellationToken)
        {
            var topic = _topics.FirstOrDefault(t => t.TopicName == topicName);
            if (topic == null)
            {
                topic = new InMemoryTopic { TopicName = topicName };
                _topics.Add(topic);
            }

            var t = client.GetTopic(topicName, Constants.InMemoryBrokername);
            if(t == null)
            {
                t = client.AddTopic(topicName, Constants.InMemoryBrokername);
            }

            var activeSession = client.GetActiveSession(sessionId);
            var subscription = new InMemoryTopicSubscription { Session = activeSession, Offset = t.Offset, ClientId = client.ClientId };
            topic.Subscriptions.Add(subscription);
            topic.CloudEventReceived += (e, o) => CloudEventReceived(e, o);
            foreach(var evt in topic.CloudEvts)
            {
                CloudEventReceived(this, new CloudEventArgs(topic.TopicName, Constants.InMemoryBrokername, evt, client.ClientId, activeSession));
            }

            return Task.CompletedTask;
        }

        public void Commit(string topicName, Client client, string sessionId, int nbEvts)
        {
            var topic = _topics.FirstOrDefault(t => t.TopicName == topicName);
            if (topic == null)
            {
                return;
            }

            var subscription = topic.Subscriptions.FirstOrDefault(s => s.ClientId == client.ClientId);
            if (subscription == null)
            {
                return;
            }

            subscription.Offset += nbEvts;
        }

        public Task Unsubscribe(string topic, Client client, string sessionId, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
    }
}
