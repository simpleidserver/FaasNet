﻿using CloudNative.CloudEvents.SystemTextJson;
using FaasNet.EventMesh.Sink.Stores;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FaasNet.EventMesh.Sink.AMQP
{
    public class AMQPSinkJob : BaseSinkJob
    {
        private const string TOPIC_NAME = "#";
        private IConnection _connection;
        private readonly EventMeshSinkAMQPOptions _options;
        private SubscriptionRecord _subscriptionRecord;

        public AMQPSinkJob(ISubscriptionStore subscriptionStore, IOptions<EventMeshSinkAMQPOptions> amqpOptions, IOptions<SinkOptions> seedOptions) : base(subscriptionStore, seedOptions)
        {
            _options = amqpOptions.Value;
        }

        protected override string JobId => _options.JobId;

        protected override async Task Subscribe(CancellationToken cancellationToken)
        {
            var offset = await SubscriptionStore.GetOffset(JobId, TOPIC_NAME, cancellationToken);
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
            channel.QueueBind(queue, _options.TopicName, TOPIC_NAME);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (s, evt) => Handle(s, evt, cancellationToken);
            channel.BasicQos(0, 100, false);
            var consumerTag = channel.BasicConsume(queue, false, string.Empty, new Dictionary<string, object> { { "x-stream-offset", offset } }, consumer);
            _subscriptionRecord = new SubscriptionRecord { Channel = channel, ConsumerTag = consumerTag };
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
            connectionFactory.HostName = _options.AMQPHostName;
            connectionFactory.Port = _options.AMQPPort;
            connectionFactory.UserName = _options.AMQPUserName;
            connectionFactory.Password = _options.AMQPPassword;
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
            await SubscriptionStore.IncrementOffset(JobId, TOPIC_NAME, cancellationToken);
        }
    }
}
