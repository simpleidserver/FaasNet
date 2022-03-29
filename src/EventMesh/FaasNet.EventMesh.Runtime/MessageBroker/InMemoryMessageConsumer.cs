using FaasNet.EventMesh.Runtime.Events;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.MessageBroker
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

        public Task Subscribe(string topicName, Models.Client client, string sessionId, CancellationToken cancellationToken)
        {
            var topic = _topics.FirstOrDefault(t => t.TopicName == topicName);
            if (topic == null)
            {
                topic = new InMemoryTopic { TopicName = topicName };
                topic.CloudEventReceived += (e, o) =>
                {
                    CloudEventReceived(e, o);
                };
                _topics.Add(topic);
            }

            var clientTopic = client.GetTopic(topicName, Constants.InMemoryBrokername);
            if(clientTopic == null)
            {
                clientTopic = client.AddTopic(topicName, Constants.InMemoryBrokername);
            }

            var activeSession = client.GetActiveSession(sessionId);
            var subscription = new InMemoryTopicSubscription { Session = activeSession, Offset = clientTopic.Offset, ClientId = client.ClientId };
            topic.Subscriptions.Add(subscription);
            foreach(var evt in topic.CloudEvts.Skip(clientTopic.Offset))
            {
                CloudEventReceived(this, new CloudEventArgs(topic.TopicName, Constants.InMemoryBrokername, evt, client.ClientId, activeSession));
            }

            return Task.CompletedTask;
        }

        public void Commit(string topicName, Models.Client client, string sessionId, int nbEvts)
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

        public Task Unsubscribe(string topic, Models.Client client, string sessionId, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }
    }
}
