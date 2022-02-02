using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Linq;

namespace EventMesh.Runtime.RabbitMQ
{
    public class RabbitMQConsumer : IMessageConsumer
    {
        private readonly RabbitMQOptions _options;
        private readonly List<RabbitMQSubscriptionRecord> _records;
        private IConnection _connection;

        public RabbitMQConsumer(IOptions<RabbitMQOptions> options)
        {
            _options = options.Value;
            _records = new List<RabbitMQSubscriptionRecord>();
        }

        #region Actions

        public void Start()
        {
            var connectionFactory = new ConnectionFactory();
            _options.ConnectionFactory(connectionFactory);
            _connection = connectionFactory.CreateConnection();
        }

        public void Stop()
        {
            foreach(var record in _records)
            {
                record.Model.Dispose();
            }

            if (_connection != null)
            {
                _connection.Dispose();
            }
        }

        public void Subscribe(string topic, string routingKey = null)
        {
            var subscription = GetSubscriptionRecord(topic, routingKey);
            if (subscription != null)
            {
                return;
            }

            var channel = _connection.CreateModel();
            var queue = channel.QueueDeclare();
            channel.QueueBind(queue, topic, routingKey);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += HandleReceiveMessage;
            channel.BasicConsume(queue, false, consumer);
            _records.Add(new RabbitMQSubscriptionRecord(topic, routingKey, channel));
        }

        public void Unsubscribe(string topic, string routingKey = null)
        {
            var subscription = GetSubscriptionRecord(topic, routingKey);
            if (subscription == null)
            {
                return;
            }

            subscription.Model.Dispose();
            _records.Remove(subscription);
        }

        public void Dispose()
        {
            Stop();
        }

        #endregion

        private void HandleReceiveMessage(object sender, BasicDeliverEventArgs e)
        {
            string sss = "";
        }

        private RabbitMQSubscriptionRecord GetSubscriptionRecord(string topic, string routingKey = null)
        {
            return _records.FirstOrDefault(r => r.Topic == topic && r.RoutingKey == routingKey);
        }
    }
}
