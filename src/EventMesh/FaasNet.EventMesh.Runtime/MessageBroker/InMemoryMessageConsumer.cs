using FaasNet.EventMesh.Runtime.Events;
using FaasNet.EventMesh.Runtime.Extensions;
using FaasNet.EventMesh.Runtime.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Runtime.MessageBroker
{
    public class InMemoryMessageConsumer : IMessageConsumer
    {
        private const int DEFAULT_TIMER_MS = 500;
        private bool _isRunning = false;
        private readonly ConcurrentBag<EventMeshCloudEvent> _cloudEvts;
        private ConcurrentBag<EventMeshSubscription> _subscriptions;
        private CancellationTokenSource _tokenSource;

        public InMemoryMessageConsumer(ConcurrentBag<EventMeshCloudEvent> cloudEvts)
        {
            _cloudEvts = cloudEvts;
        }

        public event EventHandler<CloudEventArgs> CloudEventReceived;
        public string BrokerName
        {
            get
            {
                return Constants.InMemoryBrokername;
            }
        }

        public Task Start(CancellationToken cancellationToken)
        {
            _subscriptions = new ConcurrentBag<EventMeshSubscription>();
            _isRunning = true;
            _tokenSource = new CancellationTokenSource();
            Task.Run(async () => await InternalRun());
            return Task.CompletedTask;
        }

        public Task Stop(CancellationToken cancellationToken)
        {
            _isRunning = false;
            _tokenSource.Cancel();
            return Task.CompletedTask;
        }

        public Task Subscribe(string topicName, Models.Client client, string sessionId, CancellationToken cancellationToken)
        {
            var clientTopic = client.GetTopic(topicName, Constants.InMemoryBrokername);
            if (clientTopic == null)
            {
                clientTopic = client.AddTopic(topicName, Constants.InMemoryBrokername);
            }

            var activeClientSession = client.GetActiveSession(sessionId);
            _subscriptions.Add(new EventMeshSubscription { ClientId = client.ClientId, TopicName = topicName, ClientSessionId = sessionId, Offset = clientTopic.Offset, ActiveClientSession = activeClientSession });
            return Task.CompletedTask;
        }

        public void Commit(string topicName, Models.Client client, string sessionId, int nbEvts)
        {
            var subscription = _subscriptions.First(s => s.TopicName == topicName && s.ClientSessionId == sessionId && s.ClientId == client.ClientId);
            subscription.Offset += nbEvts;
        }

        public Task Unsubscribe(string topic, Models.Client client, string sessionId, CancellationToken cancellationToken)
        {
            var subscription = _subscriptions.First(s => s.TopicName == topic && s.ClientSessionId == sessionId && s.ClientId == client.ClientId);
            _subscriptions.Remove(subscription);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            if (_isRunning)
            {
                Stop(CancellationToken.None).Wait();
            }
        }

        private async Task InternalRun()
        {
            while(!_tokenSource.IsCancellationRequested)
            {
                Thread.Sleep(DEFAULT_TIMER_MS);
                foreach(var subscription in _subscriptions)
                {
                    var filters = TopicFilterParser.Parse(subscription.TopicName).ToList();
                    var filteredCloudEvts = _cloudEvts.AsQueryable().Query(filters);
                    foreach(var cloudEvt in filteredCloudEvts)
                    {
                        CloudEventReceived(this, new CloudEventArgs(cloudEvt.Topic, Constants.InMemoryBrokername, cloudEvt.CloudEvt, subscription.ClientId, subscription.ActiveClientSession));
                    }
                }
            }
        }

        public class EventMeshSubscription
        {
            public string TopicName { get; set; }
            public string ClientId { get; set; }
            public string ClientSessionId { get; set; }
            public int Offset { get; set; }
            public ClientSession ActiveClientSession { get; set; }
        }
    }
}
