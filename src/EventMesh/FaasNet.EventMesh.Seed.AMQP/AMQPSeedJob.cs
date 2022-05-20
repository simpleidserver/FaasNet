using CloudNative.CloudEvents.SystemTextJson;
using FaasNet.EventMesh.Seed.Stores;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Seed.AMQP
{
    public class AMQPSeedJob : BaseSeedJob
    {
        private IConnection _connection;
        private readonly EventMeshSeedAMQPOptions _options;
        private SubscriptionRecord _subscriptionRecord;

        public AMQPSeedJob(ISubscriptionStore subscriptionStore, IOptions<EventMeshSeedAMQPOptions> amqpOptions, IOptions<SeedOptions> seedOptions) : base(subscriptionStore, seedOptions)
        {
            _options = amqpOptions.Value;
        }

        protected override string JobId => _options.JobId;

        protected override Task Subscribe(CancellationToken cancellationToken)
        {
            const string topicName = "#";
            var offset = SubscriptionStore.GetOffset(JobId, topicName, cancellationToken);
            var channel = BuildConnection().CreateModel();
            var queue = channel.QueueDeclare(
                "AMQPEventMesh",
                true,
                false,
                false,
                new Dictionary<string, object>
                {
                    { "x-queue-type", "stream" }
                });
            channel.QueueBind(queue, _options.TopicName, topicName);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (s, evt) => Handle(s, evt, cancellationToken);
            channel.BasicQos(0, 100, false);
            var consumerTag = channel.BasicConsume(queue, false, string.Empty, new Dictionary<string, object> { { "x-stream-offset", offset } }, consumer);
            _subscriptionRecord = new SubscriptionRecord { Channel = channel, ConsumerTag = consumerTag };
            return Task.CompletedTask;
        }

        protected override Task Unsubscribe(CancellationToken cancellationToken)
        {
            _subscriptionRecord.Channel.BasicCancel(_subscriptionRecord.ConsumerTag);
            return Task.CompletedTask;
        }

        private IConnection BuildConnection()
        {
            if (_connection != null) return _connection;
            var connectionFactory = new ConnectionFactory();
            _options.ConnectionFactory(connectionFactory);
            _connection = connectionFactory.CreateConnection();
            return _connection;
        }

        private class SubscriptionRecord
        {
            public IModel Channel { get; set; }
            public string ConsumerTag { get; set; }
        }

        private async void Handle(object sender, BasicDeliverEventArgs evt, CancellationToken cancellationToken)
        {
            var jsonEventFormatter = new JsonEventFormatter();
            var cloudEvent = evt.ToCloudEvent(jsonEventFormatter, evt.Exchange, evt.RoutingKey);
            await Publish(evt.RoutingKey, cloudEvent, cancellationToken);
        }
    }
}
