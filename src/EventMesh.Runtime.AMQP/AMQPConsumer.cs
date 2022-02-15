using CloudNative.CloudEvents.SystemTextJson;
using EventMesh.Runtime.Events;
using EventMesh.Runtime.Models;
using EventMesh.Runtime.Stores;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.AMQP
{
    public class AMQPConsumer : IMessageConsumer
    {
        private readonly List<AMQPSubscriptionRecord> _subscriptions = new List<AMQPSubscriptionRecord>();
        private readonly IBrokerConfigurationStore _brokerConfigurationStore;
        private readonly AMQPOptions _opts;
        private readonly IClientStore _clientStore;
        private readonly RuntimeOptions _runtimeOpts;
        private IConnection _connection;

        public event EventHandler<CloudEventArgs> CloudEventReceived;

        public AMQPConsumer(
            IBrokerConfigurationStore brokerConfigurationStore,
            IClientStore clientStore,
            IOptions<AMQPOptions> opts,
            IOptions<RuntimeOptions> runtimeOpts)
        {
            _opts = opts.Value;
            _brokerConfigurationStore = brokerConfigurationStore;
            _clientStore = clientStore;
            _runtimeOpts = runtimeOpts.Value;
        }

        #region Actions

        public string BrokerName
        {
            get
            {
                return _opts.BrokerName;
            }
        }

        public Task Start(CancellationToken cancellationToken)
        {
            var options = GetOptions();
            var connectionFactory = new ConnectionFactory();
            options.ConnectionFactory(connectionFactory);
            _connection = connectionFactory.CreateConnection();
            return Task.CompletedTask;
        }

        public Task Stop(CancellationToken cancellationToken)
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }

            return Task.CompletedTask;
        }

        public Task Subscribe(string topicName, Client client, string sessionId, CancellationToken cancellationToken)
        {
            var options = GetOptions();
            var activeSession = client.GetActiveSession(sessionId);
            if (activeSession.HasTopic(topicName, options.BrokerName))
            {
                return Task.CompletedTask;
            }

            var topic = client.GetTopic(topicName, options.BrokerName);
            if (topic == null)
            {
                topic = client.AddTopic(topicName, options.BrokerName);
            }

            Task.Run(() =>
            {
                Thread.Sleep(_runtimeOpts.WaitLocalSubscriptionIntervalMS);
                ListenTopic(options, topicName, topic, client.ClientId, activeSession.Id);
            });
            activeSession.SubscribeTopic(topicName, options.BrokerName);
            return Task.CompletedTask;
        }

        public Task Unsubscribe(string topicName, Client client, string sessionId, CancellationToken cancellationToken)
        {
            var options = GetOptions();
            var activeSession = client.GetActiveSession(sessionId);
            if (!activeSession.HasTopic(topicName, options.BrokerName))
            {
                return Task.CompletedTask;
            }

            var subscription = _subscriptions.First(s => s.ClientSessionId == sessionId && s.ClientId == client.ClientId && s.TopicName == topicName);
            subscription.Channel.BasicCancel(subscription.ConsumerTag);
            _subscriptions.Remove(subscription);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Stop(CancellationToken.None).Wait();
        }

        #endregion

        private void ListenTopic(AMQPOptions options, string topicName, ClientTopic topic, string clientId, string clientSessionId)
        {
            if (_subscriptions.Any(s => s.BrokerName == options.BrokerName && s.TopicName == topicName && s.ClientSessionId == clientSessionId && s.ClientId == clientId))
            {
                return;
            }

            var channel = _connection.CreateModel();
            var queue = channel.QueueDeclare(
                $"{options.QueueName}-{topicName}",
                true,
                false,
                false,
                new Dictionary<string, object>
                {
                    { "x-queue-type", "stream" }
                });
            channel.QueueBind(queue, options.TopicName, topicName);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) => ReceiveMessage(sender, clientId, clientSessionId, topicName, options.Source, options.BrokerName, e);
            // TODO : Update BasicQos.
            channel.BasicQos(0, 100, false);
            var tag = channel.BasicConsume(queue, false, string.Empty, new Dictionary<string, object> { { "x-stream-offset", topic.Offset } }, consumer);
            _subscriptions.Add(new AMQPSubscriptionRecord(topicName, options.BrokerName, clientId, clientSessionId, channel, tag));
        }

        private void ReceiveMessage(object sender, string clientId, string clientSessionId, string topicName, string source, string brokerName, BasicDeliverEventArgs e)
        {
            var jsonEventFormatter = new JsonEventFormatter();
            var model = (sender as EventingBasicConsumer).Model;
            var cloudEvent = e.ToCloudEvent(jsonEventFormatter, source, topicName);
            var client = _clientStore.GetByActiveSession(clientId, clientSessionId);
            if (client == null)
            {
                return;
            }

            var clientSession = client.GetActiveSessionByTopic(brokerName, topicName);
            CloudEventReceived(this, new CloudEventArgs(topicName, brokerName, cloudEvent, client.ClientId, clientSession));
        }

        private AMQPOptions GetOptions()
        {
           return _brokerConfigurationStore.Get(_opts.BrokerName).ToAMQPOptions();
        }
    }
}
