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
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.AMQP
{
    public class AMQPConsumer : IMessageConsumer
    {
        private readonly List<AMQPSubscriptionRecord> _records;
        private readonly IBrokerConfigurationStore _brokerConfigurationStore;
        private readonly AMQPOptions _opts;
        private IConnection _connection;

        public event EventHandler<CloudEventArgs> CloudEventReceived;

        public AMQPConsumer(
            IBrokerConfigurationStore brokerConfigurationStore,
            IOptions<AMQPOptions> opts)
        {
            _records = new List<AMQPSubscriptionRecord>();
            _opts = opts.Value;
            _brokerConfigurationStore = brokerConfigurationStore;
        }

        #region Actions

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

            var channel = ListenTopic(options, topicName, client);
            activeSession.SubscribeTopic(topicName, options.BrokerName);
            _records.Add(new AMQPSubscriptionRecord(channel, client.ClientId, activeSession, topicName));
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

            var subscription = _records.FirstOrDefault(r => r.ClientSession.Equals(activeSession) && r.TopicName == topicName);
            if (subscription == null)
            {
                return Task.CompletedTask;
            }

            subscription.Model.Dispose();
            _records.Remove(subscription);
            activeSession.UnsubscribeTopic(topicName, options.BrokerName);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Stop(CancellationToken.None).Wait();
        }

        #endregion

        private IModel ListenTopic(AMQPOptions options, string topicName, Client client)
        {
            var topic = client.GetTopic(topicName, options.BrokerName);
            if (topic == null)
            {
                topic = client.AddTopic(topicName, options.BrokerName);
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
            consumer.Received += (sender, e) => ReceiveMessage(sender, topicName, options.Source, options.BrokerName, e);
            // TODO : Update BasicQos.
            channel.BasicQos(0, 100, false);
            channel.BasicConsume(queue, false, string.Empty, new Dictionary<string, object> { { "x-stream-offset", topic.Offset } }, consumer);
            return channel;
        }

        private void ReceiveMessage(object sender, string topicName, string source, string brokerName, BasicDeliverEventArgs e)
        {
            var jsonEventFormatter = new JsonEventFormatter();
            var model = (sender as EventingBasicConsumer).Model;
            var cloudEvent = e.ToCloudEvent(jsonEventFormatter, source, topicName);
            var record = _records.FirstOrDefault(r => r.Model.Equals(model));
            if (CloudEventReceived != null)
            {
                CloudEventReceived(this, new CloudEventArgs(e.RoutingKey, brokerName, cloudEvent, record.ClientId, record.ClientSession));
            }
        }

        private AMQPOptions GetOptions()
        {
           return _brokerConfigurationStore.Get(_opts.BrokerName).ToAMQPOptions();
        }
    }
}
