using CloudNative.CloudEvents.SystemTextJson;
using EventMesh.Runtime.Events;
using EventMesh.Runtime.Models;
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
        private readonly AMQPOptions _options;
        private readonly List<AMQPSubscriptionRecord> _records;
        private IConnection _connection;

        public event EventHandler<CloudEventArgs> CloudEventReceived;

        public AMQPConsumer(IOptions<AMQPOptions> options)
        {
            _options = options.Value;
            _records = new List<AMQPSubscriptionRecord>();
        }

        #region Actions

        public Task Start(CancellationToken cancellationToken)
        {
            var connectionFactory = new ConnectionFactory();
            _options.ConnectionFactory(connectionFactory);
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

        public Task Subscribe(string topicName, Client client, CancellationToken cancellationToken)
        {
            if (client.ActiveSession.HasTopic(topicName, _options.BrokerName))
            {
                return Task.CompletedTask;
            }

            var channel = ListenTopic(topicName, client);
            client.ActiveSession.SubscribeTopic(topicName, _options.BrokerName);
            _records.Add(new AMQPSubscriptionRecord(channel, client.ClientId, client.ActiveSession, topicName));
            return Task.CompletedTask;
        }

        public Task Unsubscribe(string topicName, Client client, CancellationToken cancellationToken)
        {
            if (!client.ActiveSession.HasTopic(topicName, _options.BrokerName))
            {
                return Task.CompletedTask;
            }

            var subscription = _records.FirstOrDefault(r => r.ClientSession.Equals(client.ActiveSession) && r.TopicName == topicName);
            if (subscription == null)
            {
                return Task.CompletedTask;
            }

            subscription.Model.Dispose();
            _records.Remove(subscription);
            client.ActiveSession.UnsubscribeTopic(topicName, _options.BrokerName);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Stop(CancellationToken.None).Wait();
        }

        #endregion

        private IModel ListenTopic(string topicName, Client client)
        {
            var topic = client.GetTopic(topicName, _options.BrokerName);
            if (topic == null)
            {
                topic = client.AddTopic(topicName, _options.BrokerName);
            }

            var channel = _connection.CreateModel();
            var queue = channel.QueueDeclare(
                $"{_options.QueueName}-{topicName}",
                true,
                false,
                false,
                new Dictionary<string, object>
                {
                    { "x-queue-type", "stream" }
                });
            channel.QueueBind(queue, _options.TopicName, topicName);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) => ReceiveMessage(sender, topicName, e);
            // TODO : Update BasicQos.
            channel.BasicQos(0, 100, false);
            channel.BasicConsume(queue, false, string.Empty, new Dictionary<string, object> { { "x-stream-offset", topic.Offset } }, consumer);
            return channel;
        }

        private void ReceiveMessage(object sender, string topicName, BasicDeliverEventArgs e)
        {
            var jsonEventFormatter = new JsonEventFormatter();
            var model = (sender as EventingBasicConsumer).Model;
            var cloudEvent = e.ToCloudEvent(jsonEventFormatter, _options.Source, topicName);
            var record = _records.FirstOrDefault(r => r.Model.Equals(model));
            if (CloudEventReceived != null)
            {
                CloudEventReceived(this, new CloudEventArgs(e.RoutingKey, _options.BrokerName, cloudEvent, record.ClientId, record.ClientSession));
            }
        }
    }
}
