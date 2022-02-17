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
    public class AMQPConsumer : BaseMessageConsumer<AMQPOptions>
    {
        private readonly List<AMQPSubscriptionRecord> _subscriptions = new List<AMQPSubscriptionRecord>();
        private readonly IBrokerConfigurationStore _brokerConfigurationStore;
        private readonly AMQPOptions _opts;
        private readonly IClientStore _clientStore;
        private IConnection _connection;

        public AMQPConsumer(
            IBrokerConfigurationStore brokerConfigurationStore,
            IClientStore clientStore,
            IOptions<AMQPOptions> opts,
            IOptions<RuntimeOptions> runtimeOpts) : base(runtimeOpts)
        {
            _opts = opts.Value;
            _brokerConfigurationStore = brokerConfigurationStore;
            _clientStore = clientStore;
        }

        public override event EventHandler<CloudEventArgs> CloudEventReceived;

        #region Actions

        public override string BrokerName
        {
            get
            {
                return _opts.BrokerName;
            }
        }

        public override Task Start(CancellationToken cancellationToken)
        {
            var options = GetOptions();
            var connectionFactory = new ConnectionFactory();
            options.ConnectionFactory(connectionFactory);
            _connection = connectionFactory.CreateConnection();
            return Task.CompletedTask;
        }

        public override Task Stop(CancellationToken cancellationToken)
        {
            if (_connection != null)
            {
                _connection.Dispose();
            }

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            Stop(CancellationToken.None).Wait();
        }

        public override void Commit(string topicName, Client client, string sessionId, int nbEvts)
        {
        }

        protected override void ListenTopic(AMQPOptions options, string topicName, ClientTopic topic, string clientId, string clientSessionId)
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

        protected override void UnsubscribeTopic(string topicName, Client client, string sessionId)
        {
            var subscription = _subscriptions.First(s => s.ClientSessionId == sessionId && s.ClientId == client.ClientId && s.TopicName == topicName);
            subscription.Channel.BasicCancel(subscription.ConsumerTag);
            _subscriptions.Remove(subscription);
        }

        protected override AMQPOptions GetOptions()
        {
            return _brokerConfigurationStore.Get(_opts.BrokerName).ToAMQPOptions();
        }

        #endregion

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
    }
}
