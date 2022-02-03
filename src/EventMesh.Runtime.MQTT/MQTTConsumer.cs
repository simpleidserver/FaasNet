using CloudNative.CloudEvents;
using CloudNative.CloudEvents.Mqtt;
using CloudNative.CloudEvents.SystemTextJson;
using EventMesh.Runtime.Events;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EventMesh.Runtime.MQTT
{
    public class MQTTConsumer : IMessageConsumer
    {
        private readonly MQTTOptions _options;
        private readonly List<MQTTSubscriptionRecord> _records;
        private IMqttClient _mqttClient;

        public MQTTConsumer(IOptions<MQTTOptions> options)
        {
            _options = options.Value;
            _records = new List<MQTTSubscriptionRecord>();
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
        }

        public event EventHandler<CloudEventArgs> CloudEventReceived;

        #region Actions

        public async Task Start(CancellationToken cancellationToken)
        {
            await _mqttClient.ConnectAsync(_options.MqttClientOptions, cancellationToken);
            _mqttClient.UseApplicationMessageReceivedHandler(HandleReceiveMessage);
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            if (_mqttClient != null)
            {
                await _mqttClient.DisconnectAsync(cancellationToken);
            }
        }

        public async Task Subscribe(string topic, CancellationToken cancellationToken)
        {
            var subscription = GetSubscriptionRecord(topic);
            if (subscription != null)
            {
                return;
            }

            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
            _records.Add(new MQTTSubscriptionRecord(topic));
        }

        public async Task Unsubscribe(string topic, CancellationToken cancellationToken)
        {
            var subscription = GetSubscriptionRecord(topic);
            if (subscription == null)
            {
                return;
            }

            await _mqttClient.UnsubscribeAsync(topic);
            _records.Remove(subscription);
        }

        public void Dispose()
        {
            Stop(CancellationToken.None).Wait();
        }

        #endregion

        private void HandleReceiveMessage(MqttApplicationMessageReceivedEventArgs args)
        {
            var msg = args.ApplicationMessage;
            var jsonEventFormatter = new JsonEventFormatter();
            CloudEvent cloudEvent;
            try
            {
                cloudEvent = msg.ToCloudEvent(jsonEventFormatter);
            }
            catch
            {
                cloudEvent = new CloudEvent
                {
                    DataContentType = "application/octet-stream",
                    Data = msg.Payload
                };
            }

            if (CloudEventReceived != null)
            {
                CloudEventReceived(this, new CloudEventArgs(args.ApplicationMessage.Topic, cloudEvent));
            }
        }

        private MQTTSubscriptionRecord GetSubscriptionRecord(string topic)
        {
            return _records.FirstOrDefault(r => r.Topic == topic);
        }
    }
}
